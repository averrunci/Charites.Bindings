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
    /// The value of the specified type at the specified index.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than 0 or
    /// <paramref name="index"/> is equal to or greater than a count of the binding sources.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <typeparamref name="T"/> is not a valid type of the <see cref="BindableProperty{T}"/> at <paramref name="index"/>.
    /// </exception>
    public T GetValueAt<T>(int index)
    {
        if (index < 0 || index >= bindingSources.Count) throw new ArgumentOutOfRangeException($"The index({index}) is outside the range of valid indexes for the binding sources.");
        if (bindingSources[index] is not BindableProperty<T> bindableProperty) throw new ArgumentException($"The type({typeof(T)}) is not a valid type of the BindableProperty at the index({index}).");

        return bindableProperty.GetValue();
    }
}