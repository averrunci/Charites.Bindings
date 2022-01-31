// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System.ComponentModel;
using System.Linq.Expressions;

namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Represents a property value that provides notifications when the value is changed.
/// </summary>
/// <typeparam name="T">The type of the property value.</typeparam>
public class ObservableProperty<T> : BindableProperty<T>
{
    /// <summary>
    /// Gets or sets the property Value.
    /// </summary>
    public T Value
    {
        get => GetValue();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets the name of the value property.
    /// </summary>
    protected override string ValuePropertyName => nameof(Value);

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableProperty{T}"/> class
    /// with the specified initial value of the property.
    /// </summary>
    /// <param name="initialValue">The initial value of the property.</param>
    public ObservableProperty(T initialValue) : base(initialValue)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ObservableProperty{T}"/> class
    /// with the specified initial value of the property.
    /// </summary>
    /// <param name="initialValue">The initial value of the property.</param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public static ObservableProperty<T> Of(T initialValue) => new(initialValue);

    /// <summary>
    /// Enables the validation for the property value.
    /// </summary>
    /// <param name="selector">The selector of the property to validate.</param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public new ObservableProperty<T> EnableValidation(Expression<Func<BindableProperty<T>>> selector)
        => (ObservableProperty<T>)base.EnableValidation(selector);

    /// <summary>
    /// Enables the validation for the property value.
    /// </summary>
    /// <param name="selector">The selector of the property to validate.</param>
    /// <param name="cancelValueChangedIfInvalid">
    /// <c>true</c> if the change of the property value is canceled; otherwise <c>false</c>.
    /// </param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public new ObservableProperty<T> EnableValidation(Expression<Func<BindableProperty<T>>> selector, bool cancelValueChangedIfInvalid)
        => (ObservableProperty<T>)base.EnableValidation(selector, cancelValueChangedIfInvalid);

    /// <summary>
    /// Disables the validation for the property value.
    /// </summary>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public new ObservableProperty<T> DisableValidation()
        => (ObservableProperty<T>)base.DisableValidation();

    /// <summary>
    /// Binds the specified bindable property.
    /// </summary>
    /// <param name="source">The bindable property that is bound.</param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public new ObservableProperty<T> Bind(BindableProperty<T> source)
        => (ObservableProperty<T>)base.Bind(source);

    /// <summary>
    /// Binds the specified bindable property with the specified converter
    /// that converts the property value from the source bindable property value to
    /// the target observable property value.
    /// </summary>
    /// <typeparam name="TSource">The type of the value of the source bindable property.</typeparam>
    /// <param name="source">The bindable property that is bound.</param>
    /// <param name="converter">The converter that converts the property value.</param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has already bound another property.
    /// </exception>
    public new ObservableProperty<T> Bind<TSource>(BindableProperty<TSource> source, Func<TSource, T> converter)
        => (ObservableProperty<T>)base.Bind(source, converter);

    /// <summary>
    /// Binds the specified bindable properties with the specified converter
    /// that converts the property value from the source bindable property values to
    /// the target observable property value
    /// </summary>
    /// <param name="converter">The converter that converts the property values.</param>
    /// <param name="sources">The bindable properties that are bound.</param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has already bound another property.
    /// </exception>
    public new ObservableProperty<T> Bind(Func<MultiBindingContext, T> converter, params INotifyPropertyChanged[] sources)
        => (ObservableProperty<T>)base.Bind(converter, sources);

    /// <summary>
    /// Unbinds the bound property.
    /// </summary>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has not bound a property yet.
    /// </exception>
    public new ObservableProperty<T> Unbind()
        => (ObservableProperty<T>)base.Unbind();

    /// <summary>
    /// Binds the specified bindable property to update the other when
    /// either property value is changed.
    /// </summary>
    /// <param name="source">The bindable property that is bound.</param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has already bound another property.
    /// </exception>
    public ObservableProperty<T> BindTwoWay(BindableProperty<T> source)
    {
        if (BindingSources.Any()) throw new InvalidOperationException("The property has already bound another property.");

        Bind(source);
        source.Bind(this);

        return this;
    }

    /// <summary>
    /// Binds the specified bindable property to update the other when
    /// either property value is changed with the specified converter
    /// that converts the property value from the source bindable property value to
    /// the target observable property value and converts it from the target observable
    /// property value back to the source bindable property value.
    /// </summary>
    /// <typeparam name="TSource">The type of the value of the source bindable property.</typeparam>
    /// <param name="source">The bindable property that is bound.</param>
    /// <param name="converter">
    /// The converter that converts the source property value to the target property value.
    /// </param>
    /// <param name="backConverter">
    /// The converter that converts the target property value back to the source property value.
    /// </param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has already bound another property.
    /// </exception>
    public ObservableProperty<T> BindTwoWay<TSource>(BindableProperty<TSource> source, Func<TSource, T> converter, Func<T, TSource> backConverter)
    {
        if (BindingSources.Any()) throw new InvalidOperationException("The property has already bound another property.");

        Bind(source, converter);
        source.Bind(this, backConverter);

        return this;
    }

    /// <summary>
    /// Unbinds the specified bindable property.
    /// </summary>
    /// <typeparam name="TSource">The type of the value of the source bindable property.</typeparam>
    /// <param name="source">The bindable property that is unbound.</param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has not bound a property yet.
    /// </exception>
    public ObservableProperty<T> UnbindTwoWay<TSource>(BindableProperty<TSource> source)
    {
        if (!BindingSources.Any()) throw new InvalidOperationException("The property has not bound a property yet.");

        source.Unbind();
        Unbind();

        return this;
    }

    /// <summary>
    /// Ensures whether the property value is validated.
    /// </summary>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public new ObservableProperty<T> EnsureValidation()
        => (ObservableProperty<T>)base.EnsureValidation();

    /// <summary>
    /// Enables the delay of the property value change.
    /// </summary>
    /// <param name="delayTime">The time of delay.</param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public new ObservableProperty<T> EnableDelayValueChange(TimeSpan delayTime)
        => (ObservableProperty<T>)base.EnableDelayValueChange(delayTime);

    /// <summary>
    /// Enables the delay of the property value change
    /// </summary>
    /// <param name="delayTime">The time of delay.</param>
    /// <param name="synchronizationContext">
    /// The object used to marshal event-handler calls that are issued when to change the property value.
    /// </param>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public new ObservableProperty<T> EnableDelayValueChange(TimeSpan delayTime, SynchronizationContext synchronizationContext)
        => (ObservableProperty<T>)base.EnableDelayValueChange(delayTime, synchronizationContext);

    /// <summary>
    /// Disables the delay of the property value change.
    /// </summary>
    /// <returns>The instance of the <see cref="ObservableProperty{T}"/> class.</returns>
    public new ObservableProperty<T> DisableDelayValueChange()
        => (ObservableProperty<T>)base.DisableDelayValueChange();
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