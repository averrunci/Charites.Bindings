﻿// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Provides a content of an editable content.
/// </summary>
/// <typeparam name="T">The type of the content.</typeparam>
public interface IEditableContent<T>
{
    /// <summary>
    /// Gets the <see cref="ObservableProperty{T}"/> of the value of the content.
    /// </summary>
    ObservableProperty<T> Value { get; }
}