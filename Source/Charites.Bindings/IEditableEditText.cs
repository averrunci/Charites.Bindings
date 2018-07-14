// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings
{
    /// <summary>
    /// Provides an edit content of an editable text.
    /// </summary>
    public interface IEditableEditText : IEditableEditContent<string>
    {
        /// <summary>
        /// Gets the <see cref="ObservableProperty{T}"/> of the value
        /// that indicates whether the text is multi-text.
        /// </summary>
        ObservableProperty<bool> IsMultiLine { get; }

        /// <summary>
        /// Validates the text.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the text is valid; otherwise, <c>false</c>.
        /// </returns>
        bool Validate();
    }
}
