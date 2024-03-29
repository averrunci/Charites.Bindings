﻿// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Provides an edit content of an editable selection.
/// </summary>
public interface IEditableEditSelection : IEditableEditContent
{
    /// <summary>
    /// Gets the <see cref="ObservableProperty{T}"/> of the value
    /// that indicates whether the item is selecting.
    /// </summary>
    ObservableProperty<bool> IsSelecting { get; }
}