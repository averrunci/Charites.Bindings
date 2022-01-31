// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System.ComponentModel;
using System.Linq.Expressions;

namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Represents a property value that can bind another property value.
/// </summary>
/// <typeparam name="T">The type of the property value.</typeparam>
public class BoundProperty<T> : BindableProperty<T>
{
    /// <summary>
    /// Gets the property Value.
    /// </summary>
    public T Value => GetValue();

    /// <summary>
    /// Gets the name of the value property.
    /// </summary>
    protected override string ValuePropertyName => nameof(Value);

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundProperty{T}"/> class
    /// with the specified initial value of the property.
    /// </summary>
    /// <param name="initialValue">The initial value of the property.</param>
    public BoundProperty(T initialValue) : base(initialValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundProperty{T}"/> class
    /// with the specified bindable property.
    /// </summary>
    /// <param name="source">The bindable property that is bound.</param>
    public BoundProperty(BindableProperty<T> source) : base(source.GetValue())
    {
        Bind(source);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BoundProperty{T}"/> class
    /// with the specified initial value of the property.
    /// </summary>
    /// <param name="initialValue">The initial value of the property.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public static BoundProperty<T> Of(T initialValue) => new(initialValue);

    /// <summary>
    /// Creates a new instance of the <see cref="BoundProperty{T}"/> class
    /// with the specified bindable property.
    /// </summary>
    /// <param name="source">The bindable property that is bound.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public static BoundProperty<T> By(BindableProperty<T> source) => new(source);

    /// <summary>
    /// Creates a new instance of the <see cref="BoundProperty{T}"/> class
    /// with the specified bindable property with the specified converter
    /// that converts the property value from the source bindable property value to
    /// the target bindable property value.
    /// </summary>
    /// <typeparam name="TSource">The type of the value of the source bindable property.</typeparam>
    /// <param name="source">The bindable property that is bound.</param>
    /// <param name="converter">The converter that converts the property value.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public static BoundProperty<T> By<TSource>(BindableProperty<TSource> source, Func<TSource, T> converter)
        => Of(converter(source.GetValue())).Bind(source, converter);

    /// <summary>
    /// Creates a new instance of the <see cref="BoundProperty{T}"/> class
    /// with the specified bindable properties with the specified converter
    /// that converts the property value from the source bindable property values to
    /// the target bindable property value
    /// </summary>
    /// <param name="converter">The converter that converts the property values.</param>
    /// <param name="sources">The bindable properties that are bound.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public static BoundProperty<T> By(Func<MultiBindingContext, T> converter, params INotifyPropertyChanged[] sources)
        => Of(converter(new MultiBindingContext(sources))).Bind(converter, sources);

    /// <summary>
    /// Enables the validation for the property value.
    /// </summary>
    /// <param name="selector">The selector of the property to validate.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public new BoundProperty<T> EnableValidation(Expression<Func<BindableProperty<T>>> selector)
        => (BoundProperty<T>)base.EnableValidation(selector);

    /// <summary>
    /// Enables the validation for the property value.
    /// </summary>
    /// <param name="selector">The selector of the property to validate.</param>
    /// <param name="cancelValueChangedIfInvalid">
    /// <c>true</c> if the change of the property value is canceled; otherwise <c>false</c>.
    /// </param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public new BoundProperty<T> EnableValidation(Expression<Func<BindableProperty<T>>> selector, bool cancelValueChangedIfInvalid)
        => (BoundProperty<T>)base.EnableValidation(selector, cancelValueChangedIfInvalid);

    /// <summary>
    /// Disables the validation for the property value.
    /// </summary>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public new BoundProperty<T> DisableValidation()
        => (BoundProperty<T>)base.DisableValidation();

    /// <summary>
    /// Binds the specified bindable property.
    /// </summary>
    /// <param name="source">The bindable property that is bound.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public new BoundProperty<T> Bind(BindableProperty<T> source)
        => (BoundProperty<T>)base.Bind(source);

    /// <summary>
    /// Binds the specified bindable property with the specified converter
    /// that converts the property value from the source bindable property value to
    /// the target bindable property value.
    /// </summary>
    /// <typeparam name="TSource">The type of the value of the source bindable property.</typeparam>
    /// <param name="source">The bindable property that is bound.</param>
    /// <param name="converter">The converter that converts the property value.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has already bound another property.
    /// </exception>
    public new BoundProperty<T> Bind<TSource>(BindableProperty<TSource> source, Func<TSource, T> converter)
        => (BoundProperty<T>)base.Bind(source, converter);

    /// <summary>
    /// Binds the specified bindable properties with the specified converter
    /// that converts the property value from the source bindable property values to
    /// the target bindable property value
    /// </summary>
    /// <param name="converter">The converter that converts the property values.</param>
    /// <param name="sources">The bindable properties that are bound.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has already bound another property.
    /// </exception>
    public new BoundProperty<T> Bind(Func<MultiBindingContext, T> converter, params INotifyPropertyChanged[] sources)
        => (BoundProperty<T>)base.Bind(converter, sources);

    /// <summary>
    /// Unbinds the bound property.
    /// </summary>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    /// <exception cref="InvalidOperationException">
    /// The property has not bound a property yet.
    /// </exception>
    public new BoundProperty<T> Unbind()
        => (BoundProperty<T>)base.Unbind();

    /// <summary>
    /// Ensures whether the property value is validated.
    /// </summary>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public new BoundProperty<T> EnsureValidation()
        => (BoundProperty<T>)base.EnsureValidation();
}

/// <summary>
/// Provides extension methods of the <see cref="BoundProperty{T}"/> class.
/// </summary>
public static class BoundPropertyExtensions
{
    /// <summary>
    /// Converts the specified value to the bound property.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="target">The object that is converted to the bound property.</param>
    /// <returns>The instance of the <see cref="BoundProperty{T}"/> class.</returns>
    public static BoundProperty<T> ToBoundProperty<T>(this T target) => BoundProperty<T>.Of(target);
}