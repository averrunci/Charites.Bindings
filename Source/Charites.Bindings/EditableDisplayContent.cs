﻿// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Represents a display content of an editable content.
/// </summary>
/// <typeparam name="T">The type of the content.</typeparam>
public class EditableDisplayContent<T> : IEditableDisplayContent<T>
{
    /// <summary>
    /// Occurs when an edit is started.
    /// </summary>
    public event EventHandler? EditStarted;

    /// <summary>
    /// Gets the <see cref="ObservableProperty{T}"/> of the value that indicates whether the content is editable.
    /// </summary>
    public ObservableProperty<bool> IsEditable { get; } = new(false);

    /// <summary>
    /// Gets the <see cref="ObservableProperty{T}"/> of the value of the content.
    /// </summary>
    public ObservableProperty<T> Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableProperty{T}"/> class
    /// with the specified content value.
    /// </summary>
    /// <param name="value">The value of the display content.</param>
    public EditableDisplayContent(T value)
    {
        Value = new ObservableProperty<T>(value);
    }

    /// <summary>
    /// Starts an edit of the content.
    /// </summary>
    public void StartEdit()
    {
        if (!IsEditable.Value) return;

        OnEditStarted(EventArgs.Empty);
    }

    /// <summary>
    /// Raises the <see cref="EditStarted"/> event with the specified event data.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnEditStarted(EventArgs e) => EditStarted?.Invoke(this, e);
}