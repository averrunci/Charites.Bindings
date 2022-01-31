// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification("EditableTextProperty Spec")]
class EditableTextPropertySpec : FixtureSteppable
{
    EditableTextProperty EditableText { get; }

    string InitialText => "Initial";
    string EditedText => "Edited";

    [Background("an editable text with the initial text value is ready")]
    public EditableTextPropertySpec()
    {
        EditableText = InitialText.ToEditableTextProperty();
    }

    [Example("Completes the edit")]
    void Ex01()
    {
        Expect("the value of the editable text should be the initial text value", () => EditableText.Value.Value == InitialText);
        When("the edit is started", () => ((IEditableDisplayContent)EditableText.Content.Value).StartEdit());
        When("the value of the content is changed", () => EditableText.Content.Value.Value.Value = EditedText);
        When("the edit is completed", () => ((IEditableEditContent)EditableText.Content.Value).CompleteEdit());
        Then("the value of the editable text should be the changed value", () => EditableText.Value.Value == EditedText);
    }

    [Example("Cancels the edit")]
    void Ex02()
    {
        Expect("the value of the editable text should be the initial text value", () => EditableText.Value.Value == InitialText);
        When("the edit is started", () => ((IEditableDisplayContent)EditableText.Content.Value).StartEdit());
        When("the value of the content is changed", () => EditableText.Content.Value.Value.Value = EditedText);
        When("the edit is canceled", () => ((IEditableEditContent)EditableText.Content.Value).CancelEdit());
        Then("the value of the editable text should no be the changed", () => EditableText.Value.Value == InitialText);
    }
}