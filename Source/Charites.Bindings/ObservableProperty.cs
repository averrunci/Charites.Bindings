// Copyright (C) 2018 Fievus
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

namespace Charites.Windows.Mvc.Bindings
{
    /// <summary>
    /// Represents a property value that provides notifications when the value is changed.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public class ObservableProperty<T> : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
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

        private static readonly PropertyChangedEventArgs ValueChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
        private static readonly PropertyChangedEventArgs HasErrorChangedEventArgs = new PropertyChangedEventArgs(nameof(HasErrors));
        private static readonly PropertyChangedEventArgs ErrorChangedEventArgs = new PropertyChangedEventArgs(nameof(Error));
        private static readonly PropertyChangedEventArgs ErrorsChangedEventArgs = new PropertyChangedEventArgs(nameof(Errors));

        private IEnumerable<string> validationErrors = Enumerable.Empty<string>();
        private IEnumerable<ValidationAttribute> validations = Enumerable.Empty<ValidationAttribute>();

        private string assignedPropertyName;
        private string displayName;

        private bool cancelValueChangedIfInvalid;

        private readonly List<BindingSourceContext> bindingSources = new List<BindingSourceContext>();

        /// <summary>
        /// Gets or sets the property Value.
        /// </summary>
        public T Value
        {
            get => value;
            set
            {
                if (Equals(this.value, value)) return;

                var e = new PropertyValueChangingEventArgs<T>(ValueChangedEventArgs.PropertyName, this.value, value);
                OnPropertyValueChanging(e);

                if (!e.CanChangePropertyValue) return;

                Validate(value);
                if (cancelValueChangedIfInvalid && HasErrors) return;

                this.value = value;

                OnPropertyChanged(ValueChangedEventArgs);
                OnPropertyValueChanged(new PropertyValueChangedEventArgs<T>(e.PropertyName, e.OldValue, e.NewValue));
            }
        }
        private T value;

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
        /// Initializes a new instance of the <see cref="ObservableProperty{T}"/> class.
        /// </summary>
        public ObservableProperty() : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty{T}"/> class
        /// with the specified initial value of the property.
        /// </summary>
        /// <param name="initialValue">The initial value of the property.</param>
        public ObservableProperty(T initialValue)
        {
            value = initialValue;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ObservableProperty{T}"/> class
        /// with the specified initial value of the property.
        /// </summary>
        /// <param name="initialValue">The initial value of the property.</param>
        /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
        public static ObservableProperty<T> Of(T initialValue) => new ObservableProperty<T>(initialValue);

        /// <summary>
        /// Enables the validation for the property value.
        /// </summary>
        /// <param name="selector">The selector of the property to validate.</param>
        /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selector"/> is <c>null</c>.
        /// </exception>
        public ObservableProperty<T> EnableValidation(Expression<Func<ObservableProperty<T>>> selector)
            => EnableValidation(selector ?? throw new ArgumentNullException(nameof(selector)), false);

        /// <summary>
        /// Enables the validation for the property value.
        /// </summary>
        /// <param name="selector">The selector of the property to validate.</param>
        /// <param name="cancelValueChangedIfInvalid">
        /// <c>true</c> if the change of the property value is canceled; otherwise <c>false</c>.
        /// </param>
        /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selector"/> is <c>null</c>.
        /// </exception>
        public ObservableProperty<T> EnableValidation(Expression<Func<ObservableProperty<T>>> selector, bool cancelValueChangedIfInvalid)
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
        /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
        public ObservableProperty<T> DisableValidation()
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
        /// Binds the specified observable property.
        /// </summary>
        /// <param name="source">The observable property that is bound.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <c>null</c>.
        /// </exception>
        public void Bind(ObservableProperty<T> source)
        {
            Bind<T>(source ?? throw new ArgumentNullException(nameof(source)), t => t);
        }

        /// <summary>
        /// Binds the specified observable property with the specified converter
        /// that converts the property value from the source observable property value to
        /// the target observable property value.
        /// </summary>
        /// <typeparam name="TSource">The type of the value of the source observable property.</typeparam>
        /// <param name="source">The observable property that is bound.</param>
        /// <param name="converter">The converter that converts the property value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The property has already bound another property.
        /// </exception>
        public void Bind<TSource>(ObservableProperty<TSource> source, Func<TSource, T> converter)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            if (bindingSources.Any()) throw new InvalidOperationException("The property has already bound another property.");

            bindingSources.Add(new BindingSourceContext(source, (s, e) =>
            {
                if (!(s is ObservableProperty<TSource> sourceProperty)) return;
                if (e.PropertyName != ValueChangedEventArgs.PropertyName) return;

                Value = converter(sourceProperty.Value);
            }));
            bindingSources.ForEach(bindingSource => bindingSource.AddHandler());
            Value = converter(source.Value);
        }

        /// <summary>
        /// Binds the specified observable properties with the specified converter
        /// that converts the property value from the source observable property values to
        /// the target observable property value
        /// </summary>
        /// <param name="converter">The converter that converts the property values.</param>
        /// <param name="sources">The observable properties that are bound.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sources"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// the property has already bound another property.
        /// </exception>
        public void Bind(Func<MultiBindingContext, T> converter, params INotifyPropertyChanged[] sources)
        {
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            if (bindingSources.Any()) throw new InvalidOperationException("The property has already bound another property.");

            var context = new MultiBindingContext(sources);
            bindingSources.AddRange(sources.Select(source => new BindingSourceContext(source, (s, e) =>
            {
                if (e.PropertyName != ValueChangedEventArgs.PropertyName) return;

                Value = converter(context);
            })));
            bindingSources.ForEach(bindingSource => bindingSource.AddHandler());
            Value = converter(context);
        }

        /// <summary>
        /// Unbinds the bound property.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The property has not bound a property yet.
        /// </exception>
        public void Unbind()
        {
            if (!bindingSources.Any()) throw new InvalidOperationException("The property has not bound a property yet.");

            bindingSources.ForEach(bindingSource => bindingSource.RemoveHandler());
            bindingSources.Clear();
        }

        /// <summary>
        /// Binds the specified observable property to update the other when
        /// either property value is changed.
        /// </summary>
        /// <param name="source">The observable property that is bound.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The property has already bound another property.
        /// </exception>
        public void BindTwoWay(ObservableProperty<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (bindingSources.Any()) throw new InvalidOperationException("The property has already bound another property.");

            Bind(source);
            source.Bind(this);
        }

        /// <summary>
        /// Binds the specified observable property to update the other when
        /// either property value is changed with the specified converter
        /// that converts the property value from the source observable property value to
        /// the target observable property value and converts it from the target observable
        /// property value back to the source observable property value.
        /// </summary>
        /// <typeparam name="TSource">The type of the value of the source observable property.</typeparam>
        /// <param name="source">The observable property that is bound.</param>
        /// <param name="converter">
        /// The converter that converts the source property value to the target property value.
        /// </param>
        /// <param name="backConverter">
        /// The converter that converts the target property value back to the source property value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="backConverter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The property has already bound another property.
        /// </exception>
        public void BindTwoWay<TSource>(ObservableProperty<TSource> source, Func<TSource, T> converter, Func<T, TSource> backConverter)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            if (backConverter == null) throw new ArgumentNullException(nameof(backConverter));
            if (bindingSources.Any()) throw new InvalidOperationException("The property has already bound another property.");

            Bind(source, converter);
            source.Bind(this, backConverter);
        }

        /// <summary>
        /// Unbinds the specified observable property.
        /// </summary>
        /// <typeparam name="TSource">The type of the value of the source observable property.</typeparam>
        /// <param name="source">The observable property that is unbound.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The property has not bound a property yet.
        /// </exception>
        public void UnbindTwoWay<TSource>(ObservableProperty<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (!bindingSources.Any()) throw new InvalidOperationException("The property has not bound a property yet.");

            source.Unbind();
            Unbind();
        }

        /// <summary>
        /// Ensures whether the property value is validated.
        /// </summary>
        public void EnsureValidation()
        {
            if (Validation.IsValidated) return;

            Validate(Value);
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
        public override bool Equals(object obj) => obj is ObservableProperty<T> other && Equals(other.value, value);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A has code of the current object.</returns>
        public override int GetHashCode() => value?.GetHashCode() ?? 0;

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
            OnErrorsChanged(new DataErrorsChangedEventArgs(ValueChangedEventArgs.PropertyName));
            OnPropertyChanged(HasErrorChangedEventArgs);
            OnPropertyChanged(ErrorChangedEventArgs);
            OnPropertyChanged(ErrorsChangedEventArgs);
        }

        /// <summary>
        /// Clears validation errors.
        /// </summary>
        protected virtual void ClearValidationErrors()
        {
            validationErrors = Enumerable.Empty<string>();
            OnErrorsChanged(new DataErrorsChangedEventArgs(ValueChangedEventArgs.PropertyName));
            OnPropertyChanged(HasErrorChangedEventArgs);
            OnPropertyChanged(ErrorChangedEventArgs);
            OnPropertyChanged(ErrorsChangedEventArgs);
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
            if (Validator.TryValidateValue(e.Value, new ValidationContext(this) { MemberName = assignedPropertyName, DisplayName = displayName }, results, validations)) return;

            e.AddRange(results);
        }

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

    /// <summary>
    /// Provides extension methods of the <see cref="ObservableProperty{T}"/> class.
    /// </summary>
    public static class ObservablePropertyExtensions
    {
        /// <summary>
        /// Converts the specified value to the observable property.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="target">The object that is converted to the observable property.</param>
        /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
        public static ObservableProperty<T> ToObservableProperty<T>(this T target) => ObservableProperty<T>.Of(target);
    }
}
