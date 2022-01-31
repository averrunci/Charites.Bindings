// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Edit start")]
class EditableContentPropertySpec_EditStart : EditableContentPropertySpecContext
{
    [Example("Starts the edit by changing the IsEditing to true")]
    void Ex01()
    {
        Expect("the content of the editable content should be the display content", () => EditableContent.Content.Value == DisplayContent);
        Expect("the IsEditing should be false", () => !EditableContent.IsEditing.Value);

        When("the IsEditing is set to true", () => EditableContent.IsEditing.Value = true);
        AssertEditing(true);

        AssertEventRaised(true, false, false);
    }

    [Example("Starts the edit after the edit is started")]
    void Ex02()
    {
        When("the edit is started", () => DisplayContent.StartEdit());
        AssertEditing(true);

        ClearEventRaised();

        When("the edit is started", () => DisplayContent.StartEdit());
        AssertEditing(true);

        AssertEventRaised(false, false, false);
    }

    [Example("Starts the edit when IsEditable is false")]
    void Ex03()
    {
        When("the IsEditable is set to false", () => EditableContent.IsEditable.Value = false);
        When("the edit is started", () => DisplayContent.StartEdit());
        AssertEditing(false);

        AssertEventRaised(false, false, false);
    }

    [Example("Starts the edit by changing the IsEditable to true")]
    void Ex04()
    {
        When("the IsEditable is set to false", () => EditableContent.IsEditable.Value = false);
        When("the IsEditable is set to true", () => EditableContent.IsEditing.Value = true);
        AssertEditing(false);

        AssertEventRaised(false, false, false);
    }
}