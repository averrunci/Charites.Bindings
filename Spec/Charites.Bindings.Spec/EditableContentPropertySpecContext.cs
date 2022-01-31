// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

class EditableContentPropertySpecContext : FixtureSteppable
{
    protected EditableContentProperty<object?> EditableContent { get; }

    protected EditableDisplayContent<object?> DisplayContent { get; } = new(null);
    protected EditableEditContent<object?> EditContent { get; } = new(null);

    bool EditStartedRaised { get; set; }
    bool EditCompletedRaised { get; set; }
    bool EditCanceledRaised { get; set; }

    public EditableContentPropertySpecContext()
    {
        EditableContent = new EditableContentProperty<object?>(DisplayContent, EditContent);

        EditableContent.EditStarted += (_, e) => EditStartedRaised = e.DisplayContent == DisplayContent.Value.Value && e.EditContent == EditContent.Value.Value;
        EditableContent.EditCompleted += (_, e) => EditCompletedRaised = e.DisplayContent == DisplayContent.Value.Value && e.EditContent == EditContent.Value.Value;
        EditableContent.EditCanceled += (_, e) => EditCanceledRaised = e.DisplayContent == DisplayContent.Value.Value && e.EditContent == EditContent.Value.Value;
    }

    protected void AssertEditing(bool expectedEditing)
    {
        if (expectedEditing)
        {
            Then("the content of the editable content should be the edit content", () => EditableContent.Content.Value == EditContent);
            Then("the IsEditing should be true", () => EditableContent.IsEditing.Value);
        }
        else
        {
            Then("the content of the editable content should be the display content", () => EditableContent.Content.Value == DisplayContent);
            Then("the IsEditing should be false", () => !EditableContent.IsEditing.Value);
        }
    }

    protected void AssertEventRaised(bool expectedEditStartedRaised, bool expectedEditCompletedRaised, bool expectedEditCanceledRaised)
    {
        Then($"the EditStarted event should{(expectedEditStartedRaised ? string.Empty : " not")} be raised", () => EditStartedRaised == expectedEditStartedRaised);
        Then($"the EditCompleted event should{(expectedEditCompletedRaised ? string.Empty : " not")} be raised", () => EditCompletedRaised == expectedEditCompletedRaised);
        Then($"the EditCanceled event should{(expectedEditCanceledRaised ? string.Empty : " not")} be raised", () => EditCanceledRaised == expectedEditCanceledRaised);
    }

    protected void ClearEventRaised() => EditStartedRaised = EditCompletedRaised = EditCanceledRaised = false;
}