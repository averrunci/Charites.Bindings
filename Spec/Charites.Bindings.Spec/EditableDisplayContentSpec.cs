// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification("EditableDisplayContent Spec")]
class EditableDisplayContentSpec : FixtureSteppable
{
    EditableDisplayContent<object?> DisplayContent { get; set; } = new(new object());

    bool EditStartedRaised { get; set; }

    public EditableDisplayContentSpec()
    {
        DisplayContent.EditStarted += (_, _) => EditStartedRaised = true;
    }

    [Example("When the IsEditable property is true")]
    void Ex01()
    {
        When("the IsEditable property is set to true", () => DisplayContent.IsEditable.Value = true);
        When("the edit is started", () => DisplayContent.StartEdit());
        Then("the EditStarted event should be raised", () => EditStartedRaised);
    }

    [Example("When the IsEditable property is false")]
    void Ex02()
    {
        When("the IsEditable property is set to false", () => DisplayContent.IsEditable.Value = false);
        When("the edit is started", () => DisplayContent.StartEdit());
        Then("the EditStarted event should not be raised", () => !EditStartedRaised);
    }

    [Example("When a value specified to the constructor is null")]
    void Ex03()
    {
        When("the value that is null is specified to the constructor", () => DisplayContent = new EditableDisplayContent<object?>(null));
        Then("the value of the display content should be null without throwing an exception", () => DisplayContent.Value.Value == null);
    }
}