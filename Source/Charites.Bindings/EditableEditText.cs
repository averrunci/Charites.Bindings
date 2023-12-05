// Copyright (C) 2022-2023 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

internal sealed class EditableEditText(Func<string, bool>? validator, bool multiLine) : EditableEditContent<string>(string.Empty), IEditableEditText
{
    public ObservableProperty<bool> IsMultiLine { get; } = multiLine.ToObservableProperty();

    private Func<string, bool>? Validator { get; } = validator;

    public bool Validate() => Validator?.Invoke(Value.Value) ?? true;
}