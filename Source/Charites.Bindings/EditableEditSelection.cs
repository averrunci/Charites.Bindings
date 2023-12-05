// Copyright (C) 2022-2023 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

internal sealed class EditableEditSelection<T>(T selectedValue, IEnumerable<T> selectionItems) : EditableEditContent<T>(selectedValue), IEditableEditSelection
{
    public ObservableProperty<bool> IsSelecting { get; } = new(false);
    public IEnumerable<T> SelectionItems { get; } = selectionItems;
}