// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System.ComponentModel;

namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Represents a context of the multi binding.
/// </summary>
public sealed class MultiBindingContext
{
    private readonly List<INotifyPropertyChanged> bindingSources;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiBindingContext"/> class
    /// with the specified binding sources that need to be <see cref="BindableProperty{T}"/>.
    /// </summary>
    /// <param name="sources">The binding sources that need to be <see cref="BindableProperty{T}"/></param>
    public MultiBindingContext(IEnumerable<INotifyPropertyChanged> sources)
    {
        bindingSources = new List<INotifyPropertyChanged>(sources);
    }

    /// <summary>
    /// Gets the value of the specified type at the specified index.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="index">The index of the binding source.</param>
    /// <returns>
    /// The value of the specified type at the specified index or
    /// the default value of the specified type if the binding source
    /// at the specified index does not exist or is not <see cref="BindableProperty{T}"/>.
    /// </returns>
    public T? GetValueAt<T>(int index)
    {
        if (index < 0 || index >= bindingSources.Count) return default;

        return bindingSources[index] is BindableProperty<T> bindableProperty ? bindableProperty.GetValue() : default;
    }
}