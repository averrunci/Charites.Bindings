﻿// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Represents a property value that can be edited with a dedicated content.
/// </summary>
/// <typeparam name="T">The type of the content.</typeparam>
public class EditableContentProperty<T>
{
    /// <summary>
    /// Occurs when an edit is completed.
    /// </summary>
    public event EditableContentEventHandler<T>? EditStarted;

    /// <summary>
    /// Occurs when an edit is completed.
    /// </summary>
    public event EditableContentEventHandler<T>? EditCompleted;

    /// <summary>
    /// Occurs when an edit is canceled.
    /// </summary>
    public event EditableContentEventHandler<T>? EditCanceled;

    /// <summary>
    /// Gets the <see cref="ObservableProperty{T}"/> of the value that indicates whether the content is editable.
    /// </summary>
    public ObservableProperty<bool> IsEditable { get; } = new(true);

    /// <summary>
    /// Gets the <see cref="ObservableProperty{T}"/> of the value that indicates whether the content is editing.
    /// </summary>
    public ObservableProperty<bool> IsEditing { get; } = new(false);

    /// <summary>
    /// Gets the <see cref="ObservableProperty{T}"/> of the value of the display content.
    /// </summary>
    public ObservableProperty<T> Value => DisplayContent.Value;

    /// <summary>
    /// Gets the <see cref="ObservableProperty{T}"/> of the value of the current content.
    /// </summary>
    public ObservableProperty<IEditableContent<T>> Content { get; }

    /// <summary>
    /// Gets the display content.
    /// </summary>
    protected IEditableDisplayContent<T> DisplayContent { get; }

    /// <summary>
    /// Gets the edit content.
    /// </summary>
    protected IEditableEditContent<T> EditContent { get; }

    private bool isEditStarted;
    private bool isEditCompleted;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableContentProperty{T}"/> class
    /// with the specified display content and edit content.
    /// </summary>
    /// <param name="displayContent">The display content.</param>
    /// <param name="editContent">The edit content.</param>
    public EditableContentProperty(IEditableDisplayContent<T> displayContent, IEditableEditContent<T> editContent)
    {
        DisplayContent = displayContent;
        EditContent = editContent;

        DisplayContent.IsEditable.BindTwoWay(IsEditable);
        DisplayContent.EditStarted += OnDisplayContentEditStarted;

        EditContent.EditCompleted += OnEditContentEditCompleted;
        EditContent.EditCanceled += OnEditContentEditCanceled;

        IsEditable.PropertyValueChanged += OnIsEditableChanged;
        IsEditing.PropertyValueChanged += OnIsEditingChanged;

        Content = new ObservableProperty<IEditableContent<T>>(DisplayContent);
    }

    /// <summary>
    /// Changes the current content to the edit content.
    /// </summary>
    protected virtual void ToEdit() => Content.Value = EditContent;

    /// <summary>
    /// Changes the current content to the display content.
    /// </summary>
    protected virtual void ToDisplay() => Content.Value = DisplayContent;

    /// <summary>
    /// Raises the <see cref="EditStarted"/> event with the specified event data.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnEditStarted(EditableContentEventArgs<T> e) => EditStarted?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="EditCompleted"/> event with the specified event data.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnEditCompleted(EditableContentEventArgs<T> e) => EditCompleted?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="EditCanceled"/> event with the specified event data.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnEditCanceled(EditableContentEventArgs<T> e) => EditCanceled?.Invoke(this, e);

    private void OnDisplayContentEditStarted(object? sender, EventArgs e)
    {
        IsEditing.Value = true;
    }

    private void OnEditContentEditCompleted(object? sender, EventArgs e)
    {
        isEditCompleted = true;
        IsEditing.Value = false;
    }

    private void OnEditContentEditCanceled(object? sender, EventArgs e)
    {
        IsEditing.Value = false;
    }

    private void OnIsEditableChanged(object? sender, PropertyValueChangedEventArgs<bool> e)
    {
        if (!IsEditing.Value || e.NewValue) return;

        IsEditing.Value = false;
    }

    private void OnIsEditingChanged(object? sender, PropertyValueChangedEventArgs<bool> e)
    {
        if (e.NewValue)
        {
            OnIsEditingChangedToTrue();
        }
        else
        {
            OnIsEditingChangedToFalse();
        }
    }

    private void OnIsEditingChangedToTrue()
    {
        isEditCompleted = false;

        if (!IsEditable.Value)
        {
            IsEditing.Value = false;
            return;
        }

        OnEditStarted(new EditableContentEventArgs<T>(DisplayContent.Value.Value, EditContent.Value.Value));

        ToEdit();

        isEditStarted = true;
    }

    private void OnIsEditingChangedToFalse()
    {
        if (!isEditStarted) return;

        var e = new EditableContentEventArgs<T>(DisplayContent.Value.Value, EditContent.Value.Value);
        if (isEditCompleted)
        {
            OnEditCompleted(e);
        }
        else
        {
            OnEditCanceled(e);
        }

        ToDisplay();

        isEditStarted = false;
    }
}