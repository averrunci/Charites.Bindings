// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Charites.Windows.Mvc.Bindings
{
    /// <summary>
    /// Represents a base class of a property value that can bind another property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public abstract class BindableProperty<T> : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property value changes.
        /// </summary>
        public event PropertyValueChangingEventHandler<T> PropertyValueChanging;

        /// <summary>
        /// Occurs after a property value changes.
        /// </summary>
        public event PropertyValueChangedEventHandler<T> PropertyValueChanged;

        /// <summary>
        /// Occurs when to validate a property value.
        /// </summary>
        public event PropertyValueValidateEventHandler<T> PropertyValueValidate;

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entry.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private readonly PropertyValueChangeTimer<T> valueChangedTimer;

        private PropertyChangedEventArgs valueChangedEventArgs;
        private DataErrorsChangedEventArgs dataErrorsChangedEventArgs;

        private IEnumerable<string> validationErrors = Enumerable.Empty<string>();
        private IEnumerable<ValidationAttribute> validations = Enumerable.Empty<ValidationAttribute>();

        private string assignedPropertyName;
        private string displayName;

        private bool cancelValueChangedIfInvalid;


        private T value;

        /// <summary>
        /// Gets the name of the value property.
        /// </summary>
        protected abstract string ValuePropertyName { get; }

        /// <summary>
        /// Gets the binding sources.
        /// </summary>
        protected List<BindingSourceContext> BindingSources { get; } = new List<BindingSourceContext>();

        /// <summary>
        /// Gets the state of the validation for the property value.
        /// </summary>
        protected virtual ValidationState Validation { get; } = new ValidationState();

        /// <summary>
        /// Gets a value that indicates whether the value has validation errors.
        /// </summary>
        public virtual bool HasErrors => validationErrors.Any();

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public virtual string Error => string.Join(Environment.NewLine, validationErrors.ToList());

        /// <summary>
        /// Gets an enumerable of error message indicating what is wrong with this object.
        /// </summary>
        public virtual IEnumerable<string> Errors => validationErrors.ToList().AsReadOnly();

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">The name of the property whose error message to get.</param>
        /// <returns>The error message for the property. The default value is an empty string.</returns>
        protected virtual string this[string columnName] => Error;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableProperty{T}"/> class.
        /// </summary>
        protected BindableProperty() : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableProperty{T}"/> class
        /// with the specified initial value of the property.
        /// </summary>
        /// <param name="initialValue">The initial value of the property.</param>
        protected BindableProperty(T initialValue)
        {
            value = initialValue;

            valueChangedTimer = new PropertyValueChangeTimer<T>(SetValue);
        }

        /// <summary>
        /// Enables the validation for the property value.
        /// </summary>
        /// <param name="selector">The selector of the property to validate.</param>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selector"/> is <c>null</c>.
        /// </exception>
        public BindableProperty<T> EnableValidation(Expression<Func<BindableProperty<T>>> selector)
            => EnableValidation(selector ?? throw new ArgumentNullException(nameof(selector)), false);

        /// <summary>
        /// Enables the validation for the property value.
        /// </summary>
        /// <param name="selector">The selector of the property to validate.</param>
        /// <param name="cancelValueChangedIfInvalid">
        /// <c>true</c> if the change of the property value is canceled; otherwise <c>false</c>.
        /// </param>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selector"/> is <c>null</c>.
        /// </exception>
        public BindableProperty<T> EnableValidation(Expression<Func<BindableProperty<T>>> selector, bool cancelValueChangedIfInvalid)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            DisableValidation();

            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression == null) throw new ArgumentException("The body of the selector must be MemberExpression.");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null) throw new ArgumentException("The member of the body of the selector must be PropertyInfo.");

            Validation.Enable();
            validations = property.GetCustomAttributes<ValidationAttribute>(true);
            assignedPropertyName = property.Name;
            displayName = property.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.GetName() ?? assignedPropertyName;
            this.cancelValueChangedIfInvalid = cancelValueChangedIfInvalid;
            PropertyValueValidate += OnPropertyValueValidate;

            return this;
        }

        /// <summary>
        /// Disables the validation for the property value.
        /// </summary>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        public BindableProperty<T> DisableValidation()
        {
            PropertyValueValidate -= OnPropertyValueValidate;
            ClearValidationErrors();
            validations = Enumerable.Empty<ValidationAttribute>();
            assignedPropertyName = null;
            displayName = null;
            cancelValueChangedIfInvalid = false;
            Validation.Disable();

            return this;
        }

        /// <summary>
        /// Binds the specified bindable property.
        /// </summary>
        /// <param name="source">The bindable property that is bound.</param>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <c>null</c>.
        /// </exception>
        public BindableProperty<T> Bind(BindableProperty<T> source)
        {
            Bind(source ?? throw new ArgumentNullException(nameof(source)), t => t);

            return this;
        }

        /// <summary>
        /// Binds the specified bindable property with the specified converter
        /// that converts the property value from the source bindable property value to
        /// the target bindable property value.
        /// </summary>
        /// <typeparam name="TSource">The type of the value of the source bindable property.</typeparam>
        /// <param name="source">The bindable property that is bound.</param>
        /// <param name="converter">The converter that converts the property value.</param>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The property has already bound another property.
        /// </exception>
        public BindableProperty<T> Bind<TSource>(BindableProperty<TSource> source, Func<TSource, T> converter)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            if (BindingSources.Any()) throw new InvalidOperationException("The property has already bound another property.");

            BindingSources.Add(new BindingSourceContext(source, (s, e) =>
            {
                if (!(s is BindableProperty<TSource> sourceProperty)) return;
                if (e.PropertyName != ValuePropertyName) return;

                SetValue(converter(sourceProperty.GetValue()));
            }));
            BindingSources.ForEach(bindingSource => bindingSource.AddHandler());
            SetValue(converter(source.GetValue()));

            return this;
        }

        /// <summary>
        /// Binds the specified bindable properties with the specified converter
        /// that converts the property value from the source bindable property values to
        /// the target bindable property value
        /// </summary>
        /// <param name="converter">The converter that converts the property values.</param>
        /// <param name="sources">The bindable properties that are bound.</param>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sources"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// the property has already bound another property.
        /// </exception>
        public BindableProperty<T> Bind(Func<MultiBindingContext, T> converter, params INotifyPropertyChanged[] sources)
        {
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            if (BindingSources.Any()) throw new InvalidOperationException("The property has already bound another property.");

            var context = new MultiBindingContext(sources);
            BindingSources.AddRange(sources.Select(source => new BindingSourceContext(source, (s, e) =>
            {
                if (e.PropertyName != ValuePropertyName) return;

                SetValue(converter(context));
            })));
            BindingSources.ForEach(bindingSource => bindingSource.AddHandler());
            SetValue(converter(context));

            return this;
        }

        /// <summary>
        /// Unbinds the bound property.
        /// </summary>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        /// <exception cref="InvalidOperationException">
        /// The property has not bound a property yet.
        /// </exception>
        public BindableProperty<T> Unbind()
        {
            if (!BindingSources.Any()) throw new InvalidOperationException("The property has not bound a property yet.");

            BindingSources.ForEach(bindingSource => bindingSource.RemoveHandler());
            BindingSources.Clear();

            return this;
        }

        /// <summary>
        /// Ensures whether the property value is validated.
        /// </summary>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        public BindableProperty<T> EnsureValidation()
        {
            if (!Validation.IsValidated) Validate(GetValue());

            return this;
        }

        /// <summary>
        /// Enables the delay of the property value change.
        /// </summary>
        /// <param name="delayTime">The time of delay.</param>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        public BindableProperty<T> EnableDelayValueChange(TimeSpan delayTime)
            => EnableDelayValueChange(delayTime, SynchronizationContext.Current);

        /// <summary>
        /// Enables the delay of the property value change
        /// </summary>
        /// <param name="delayTime">The time of delay.</param>
        /// <param name="synchronizationContext">
        /// The object used to marshal event-handler calls that are issued when to change the property value.
        /// </param>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        public BindableProperty<T> EnableDelayValueChange(TimeSpan delayTime, SynchronizationContext synchronizationContext)
        {
            valueChangedTimer.Enable(delayTime, synchronizationContext);

            return this;
        }

        /// <summary>
        /// Disables the delay of the property value change.
        /// </summary>
        /// <returns>The instance of the <see cref="BindableProperty{T}"/> class.</returns>
        public BindableProperty<T> DisableDelayValueChange()
        {
            valueChangedTimer.Disable();

            return this;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => value == null ? string.Empty : value.ToString();

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is BindableProperty<T> other && Equals(other.value, value);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A has code of the current object.</returns>
        public override int GetHashCode() => value?.GetHashCode() ?? 0;

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <returns>The value of the property.</returns>
        protected virtual T GetValue() => value;

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="value">The value of the property.</param>
        protected virtual void SetValue(T value)
        {
            if (Equals(this.value, value)) return;

            var e = new PropertyValueChangingEventArgs<T>(ValuePropertyName, this.value, value);
            OnPropertyValueChanging(e);

            if (!e.CanChangePropertyValue) return;

            Validate(value);
            if (cancelValueChangedIfInvalid && HasErrors) return;

            valueChangedTimer.Restart(e.PropertyName, e.OldValue, e.NewValue);
        }

        private void SetValue(string propertyName, T oldValue, T newValue)
        {
            value = newValue;

            OnPropertyChanged(EnsureValueChangedEventArgs());
            OnPropertyValueChanged(new PropertyValueChangedEventArgs<T>(propertyName, oldValue, newValue));
        }

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        protected virtual void Validate(T value)
        {
            Validation.Validated();
            var validateArgs = new PropertyValueValidateEventArgs<T>(value);
            OnPropertyValueValidate(validateArgs);
            OnValidated(validateArgs.Results);
        }

        /// <summary>
        /// Handles the specified validation results after the value is validated.
        /// </summary>
        /// <param name="results">The results of the validation.</param>
        protected virtual void OnValidated(IEnumerable<ValidationResult> results)
        {
            var errors = results.Select(result => result.ErrorMessage).ToList();
            if (!validationErrors.Any() && !errors.Any()) return;

            if (errors.Any())
            {
                SetValidationErrors(errors);
            }
            else
            {
                ClearValidationErrors();
            }
        }

        /// <summary>
        /// Sets the specified validation errors.
        /// </summary>
        /// <param name="errors">The validation errors to set.</param>
        protected virtual void SetValidationErrors(IEnumerable<string> errors)
        {
            validationErrors = errors;
            OnErrorsChanged(EnsureDataErrorsChangedEventArgs());
            OnPropertyChanged(ChangedEventArgs.HasErrorsChangedEventArgs);
            OnPropertyChanged(ChangedEventArgs.ErrorChangedEventArgs);
            OnPropertyChanged(ChangedEventArgs.ErrorsChangedEventArgs);
        }

        /// <summary>
        /// Clears validation errors.
        /// </summary>
        protected virtual void ClearValidationErrors()
        {
            validationErrors = Enumerable.Empty<string>();
            OnErrorsChanged(EnsureDataErrorsChangedEventArgs());
            OnPropertyChanged(ChangedEventArgs.HasErrorsChangedEventArgs);
            OnPropertyChanged(ChangedEventArgs.ErrorChangedEventArgs);
            OnPropertyChanged(ChangedEventArgs.ErrorsChangedEventArgs);
        }

        /// <summary>
        /// Gets the validation errors for the specified property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or <c>null</c> or
        /// <see cref="string.Empty"/>.
        /// </param>
        /// <returns>The validation errors for the property.</returns>
        protected virtual IEnumerable GetErrors(string propertyName) => validationErrors;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event with the specified event data.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="PropertyValueChanging"/> event with the specified event data.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnPropertyValueChanging(PropertyValueChangingEventArgs<T> e) => PropertyValueChanging?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="PropertyValueChanged"/> event with the specified event data.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs<T> e) => PropertyValueChanged?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="PropertyValueValidate"/> event with the specified event data.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnPropertyValueValidate(PropertyValueValidateEventArgs<T> e) => PropertyValueValidate?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event with the specified event data.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e) => ErrorsChanged?.Invoke(this, e);

        private void OnPropertyValueValidate(object sender, PropertyValueValidateEventArgs<T> e)
        {
            var results = new Collection<ValidationResult>();
            if (ValidatePropertyValue(e.Value, results)) return;

            e.AddRange(results);
        }

        private bool ValidatePropertyValue(T propertyValue, ICollection<ValidationResult> results)
        {
            return Validator.TryValidateValue(propertyValue, new ValidationContext(this) { MemberName = assignedPropertyName, DisplayName = displayName }, results, validations);
        }

        private PropertyChangedEventArgs EnsureValueChangedEventArgs() => valueChangedEventArgs ?? (valueChangedEventArgs = new PropertyChangedEventArgs(ValuePropertyName));
        private DataErrorsChangedEventArgs EnsureDataErrorsChangedEventArgs() => dataErrorsChangedEventArgs ?? (dataErrorsChangedEventArgs = new DataErrorsChangedEventArgs(ValuePropertyName));

        bool INotifyDataErrorInfo.HasErrors => HasErrors;
        string IDataErrorInfo.Error => Error;
        string IDataErrorInfo.this[string columnName] => this[columnName];
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) => GetErrors(propertyName);

        /// <summary>
        /// Represents a state of a validation for the property value.
        /// </summary>
        protected class ValidationState
        {
            /// <summary>
            /// Gets a value that indicates whether the validation is enabled.
            /// </summary>
            public bool IsEnabled { get; private set; }

            /// <summary>
            /// Gets a value that indicates whether the property value is validated
            /// </summary>
            public bool IsValidated { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ValidationState"/> class.
            /// </summary>
            public ValidationState()
            {
                Disable();
            }

            /// <summary>
            /// Enables the validation for the property value.
            /// </summary>
            public void Enable()
            {
                IsEnabled = true;
                IsValidated = false;
            }

            /// <summary>
            /// Disables the validation for the property value.
            /// </summary>
            public void Disable()
            {
                IsEnabled = false;
                IsValidated = false;
            }

            /// <summary>
            /// Sets the value that indicates whether the property value is validated to <c>true</c>.
            /// </summary>
            public void Validated()
            {
                IsValidated = true;
            }
        }

        /// <summary>
        /// Represents a context of the binding source.
        /// </summary>
        protected class BindingSourceContext
        {
            /// <summary>
            /// Gets a binding source.
            /// </summary>
            protected INotifyPropertyChanged Source { get; }

            /// <summary>
            /// Gets <see cref="PropertyChangedHandler"/> of the binding source.
            /// </summary>
            protected PropertyChangedEventHandler PropertyChangedHandler { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="BindingSourceContext"/> class
            /// with the specified binding source and <see cref="PropertyChangedHandler"/>.
            /// </summary>
            /// <param name="source">The binding source.</param>
            /// <param name="propertyChangedEventHandler">
            /// <see cref="PropertyChangedHandler"/> of the binding source.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="source"/> is <c>null</c>.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="propertyChangedEventHandler"/> is <c>null</c>.
            /// </exception>
            public BindingSourceContext(INotifyPropertyChanged source, PropertyChangedEventHandler propertyChangedEventHandler)
            {
                Source = source ?? throw new ArgumentNullException(nameof(source));
                PropertyChangedHandler = propertyChangedEventHandler ?? throw new ArgumentNullException(nameof(propertyChangedEventHandler));
            }

            /// <summary>
            /// Adds <see cref="PropertyChangedHandler"/> to the binding source.
            /// </summary>
            public void AddHandler() => Source.PropertyChanged += PropertyChangedHandler;

            /// <summary>
            /// Removes <see cref="PropertyChangedHandler"/> from the binding source.
            /// </summary>
            public void RemoveHandler() => Source.PropertyChanged -= PropertyChangedHandler;
        }
    }

    internal static class ChangedEventArgs
    {
        public static readonly PropertyChangedEventArgs HasErrorsChangedEventArgs = new PropertyChangedEventArgs(nameof(BindableProperty<object>.HasErrors));
        public static readonly PropertyChangedEventArgs ErrorChangedEventArgs = new PropertyChangedEventArgs(nameof(BindableProperty<object>.Error));
        public static readonly PropertyChangedEventArgs ErrorsChangedEventArgs = new PropertyChangedEventArgs(nameof(BindableProperty<object>.Errors));
    }
}
