// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Represents a property of a text that can be edited with a dedicated content.
/// </summary>
public class EditableTextProperty : EditableContentProperty<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextProperty"/> class.
    /// </summary>
    public EditableTextProperty() : this(string.Empty, null, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextProperty"/> class
    /// with the specified initial text value.
    /// </summary>
    /// <param name="initialValue">The initial text value.</param>
    public EditableTextProperty(string initialValue) : this(initialValue, null, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextProperty"/> class
    /// with the specified validator that validates the text.
    /// </summary>
    /// <param name="validator">The function to validate the text.</param>
    public EditableTextProperty(Func<string, bool> validator) : this(string.Empty, validator, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextProperty"/> class
    /// with the specified value that indicates whether the text is multi-line.
    /// </summary>
    /// <param name="multiLine">
    /// <c>true</c> if the text is multi-line; otherwise, <c>false</c>.
    /// </param>
    public EditableTextProperty(bool multiLine) : this(string.Empty, null, multiLine)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextProperty"/> class
    /// with the specified initial text value and validator that validates the text.
    /// </summary>
    /// <param name="initialValue">The initial text value.</param>
    /// <param name="validator">The function to validate the text.</param>
    public EditableTextProperty(string initialValue, Func<string, bool> validator) : this(initialValue, validator, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextProperty"/> class
    /// with the initial text value and value that indicates whether the text is multi-line.
    /// </summary>
    /// <param name="initialValue">The initial text value.</param>
    /// <param name="multiLine">
    /// <c>true</c> if the text is multi-line; otherwise, <c>false</c>.
    /// </param>
    public EditableTextProperty(string initialValue, bool multiLine) : this(initialValue, null, multiLine)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextProperty"/> class
    /// with the specified validator that validates the text and value
    /// that indicates whether the text is multi-line.
    /// </summary>
    /// <param name="validator">The function to validate the text.</param>
    /// <param name="multiLine">
    /// <c>true</c> if the text is multi-line; otherwise, <c>false</c>.
    /// </param>
    public EditableTextProperty(Func<string, bool> validator, bool multiLine) : this(string.Empty, validator, multiLine)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableEditText"/> class
    /// with the specified initial text value, validator that validates the text, and
    /// value that indicates whether the text is multi-line.
    /// </summary>
    /// <param name="initialValue">The initial text value.</param>
    /// <param name="validator">The function to validate the text.</param>
    /// <param name="multiLine">
    /// <c>true</c> if the text is multi-line; otherwise, <c>false</c>.
    /// </param>
    public EditableTextProperty(string initialValue, Func<string, bool>? validator, bool multiLine) : base(new EditableDisplayContent<string>(initialValue), new EditableEditText(validator, multiLine))
    {
    }

    /// <summary>
    /// Sets the value from the display content to the edit content and
    /// raises the <see cref="EditableContentProperty{T}.EditStarted"/> event with the specified event data.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnEditStarted(EditableContentEventArgs<string> e)
    {
        EditContent.Value.Value = DisplayContent.Value.Value;

        base.OnEditStarted(e);
    }

    /// <summary>
    /// Sets the value from the edit content to the display content and
    /// raises the <see cref="EditableContentProperty{T}.EditCompleted"/> event with the specified event data.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnEditCompleted(EditableContentEventArgs<string> e)
    {
        DisplayContent.Value.Value = EditContent.Value.Value;

        base.OnEditCompleted(e);
    }
}

/// <summary>
/// Provides extension methods of the <see cref="EditableTextProperty"/> class.
/// </summary>
public static class EditableTextPropertyExtensions
{
    /// <summary>
    /// Converts the text value to the <see cref="EditableTextProperty"/>.
    /// </summary>
    /// <param name="value">The text value that is converted to the <see cref="EditableTextProperty"/>.</param>
    /// <returns>The instance of the <see cref="EditableTextProperty"/> class.</returns>
    public static EditableTextProperty ToEditableTextProperty(this string value) => new(value);

    /// <summary>
    /// Converts the text value to the <see cref="EditableTextProperty"/> with the specified validator
    /// that validates the text.
    /// </summary>
    /// <param name="value">The text value that is converted to the <see cref="EditableTextProperty"/>.</param>
    /// <param name="validator">The function to validate the text.</param>
    /// <returns>The instance of the <see cref="EditableTextProperty"/> class.</returns>
    public static EditableTextProperty ToEditableTextProperty(this string value, Func<string, bool> validator) => new(value, validator);

    /// <summary>
    /// Converts the text value to the <see cref="EditableTextProperty"/> with the specified value that
    /// indicates whether the text is multi-line.
    /// </summary>
    /// <param name="value">The text value that is converted to the <see cref="EditableTextProperty"/>.</param>
    /// <param name="multiLine">
    /// <c>true</c> if the text is multi-line; otherwise, <c>false</c>.
    /// </param>
    /// <returns>The instance of the <see cref="EditableTextProperty"/> class.</returns>
    public static EditableTextProperty ToEditableTextProperty(this string value, bool multiLine) => new(value, multiLine);

    /// <summary>
    /// Converts the text value to the <see cref="EditableTextProperty"/> with the specified validator
    /// that validates the text and value that indicates whether the text is multi-line.
    /// </summary>
    /// <param name="value">The text value that is converted to the <see cref="EditableTextProperty"/>.</param>
    /// <param name="validator">The function to validate the text.</param>
    /// <param name="multiLine">
    /// <c>true</c> if the text is multi-line; otherwise, <c>false</c>.
    /// </param>
    /// <returns>The instance of the <see cref="EditableTextProperty"/> class.</returns>
    public static EditableTextProperty ToEditableTextProperty(this string value, Func<string, bool> validator, bool multiLine) => new(value, validator, multiLine);
}