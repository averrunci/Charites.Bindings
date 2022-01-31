// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification("EditableSelectionProperty Spec")]
class EditableSelectionPropertySpec : FixtureSteppable
{
    EditableSelectionProperty EditableSelection { get; }

    string[] SelectionItems { get; } = { "Item1", "Item2", "Item3", "Item4", "Item5" };
    string InitialSelectedItem => "Item2";
    string SelectedItem { get; }

    [Background("an editable selection with the initial selected item is ready")]
    public EditableSelectionPropertySpec()
    {
        EditableSelection = InitialSelectedItem.ToEditableSelectionProperty(SelectionItems);
        SelectedItem = SelectionItems[3];
    }

    [Example("Completes the edit")]
    void Ex01()
    {
        Expect("the value of the editable selection should be the initial selected item", () => EditableSelection.Value.Value == InitialSelectedItem);
        When("the edit is started", () => ((IEditableDisplayContent)EditableSelection.Content.Value).StartEdit());
        When("the value of the content is selected", () => EditableSelection.Content.Value.Value.Value = SelectedItem);
        When("the edit is completed", () => ((IEditableEditContent)EditableSelection.Content.Value).CompleteEdit());
        Then("the value of the editable selection should be the selected value", () => EditableSelection.Value.Value == SelectedItem);
    }

    [Example("Cancels the edit")]
    void Ex02()
    {
        Expect("the value of the editable selection should be the initial selected item", () => EditableSelection.Value.Value == InitialSelectedItem);
        When("the edit is started", () => ((IEditableDisplayContent)EditableSelection.Content.Value).StartEdit());
        When("the value of the content is selected", () => EditableSelection.Content.Value.Value.Value = SelectedItem);
        When("the edit is canceled", () => ((IEditableEditContent)EditableSelection.Content.Value).CancelEdit());
        Then("the value of the editable selection should not be changed", () => EditableSelection.Value.Value == InitialSelectedItem);
    }
}