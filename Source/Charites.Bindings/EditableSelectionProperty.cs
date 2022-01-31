// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Represents a property value that can be selected with a dedicated content using a selection control.
/// </summary>
/// <typeparam name="T">The type of the selection item.</typeparam>
public class EditableSelectionProperty<T> : EditableContentProperty<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditableSelectionProperty{T}"/> class
    /// with the specified initial value and selection items.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="selectionItems">The selection items from which the property value is selected.</param>
    public EditableSelectionProperty(T initialValue, IEnumerable<T> selectionItems) : base(new EditableDisplayContent<T>(initialValue), new EditableEditSelection<T>(initialValue, selectionItems))
    {
    }

    /// <summary>
    /// Sets the value from the display content to the edit content and
    /// raises the <see cref="EditableContentProperty{T}.EditStarted"/> event with the specified event data.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnEditStarted(EditableContentEventArgs<T> e)
    {
        EditContent.Value.Value = DisplayContent.Value.Value;

        base.OnEditStarted(e);
    }

    /// <summary>
    /// Sets the value from the edit content to the display content and
    /// raises the <see cref="EditableContentProperty{T}.EditCompleted"/> event with the specified event data.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnEditCompleted(EditableContentEventArgs<T> e)
    {
        DisplayContent.Value.Value = EditContent.Value.Value;

        base.OnEditCompleted(e);
    }
}

/// <summary>
/// Represents a property of a text that can be selected with a dedicated content using a selection control.
/// </summary>
public class EditableSelectionProperty : EditableSelectionProperty<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditableSelectionProperty"/> class
    /// with the specified initial value and selection items.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="selectionItems">The selection items from which the text value is selected.</param>
    public EditableSelectionProperty(string initialValue, IEnumerable<string> selectionItems) : base(initialValue, selectionItems)
    {
    }
}

/// <summary>
/// Provides extension methods of the <see cref="EditableSelectionProperty{T}"/> and <see cref="EditableSelectionProperty"/> class.
/// </summary>
public static class EditableSelectionPropertyExtensions
{
    /// <summary>
    /// Converts the property value to the <see cref="EditableSelectionProperty{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the selection items.</typeparam>
    /// <param name="value">The property value that is converted to the <see cref="EditableSelectionProperty{T}"/>.</param>
    /// <param name="selectionItems">The selection items from which the property value is selected.</param>
    /// <returns>The instance of the <see cref="EditableSelectionProperty{T}"/> class.</returns>
    public static EditableSelectionProperty<T> ToEditableSelectionProperty<T>(this T value, IEnumerable<T> selectionItems) => new(value, selectionItems);

    /// <summary>
    /// Converts the text value to the <see cref="EditableSelectionProperty"/>.
    /// </summary>
    /// <param name="value">The text value that is converted to the <see cref="EditableSelectionProperty"/>.</param>
    /// <param name="selectionItems">The selection items from which the text value is selected.</param>
    /// <returns>The instance of the <see cref="EditableSelectionProperty"/> class.</returns>
    public static EditableSelectionProperty ToEditableSelectionProperty(this string value, IEnumerable<string> selectionItems) => new(value, selectionItems);
}