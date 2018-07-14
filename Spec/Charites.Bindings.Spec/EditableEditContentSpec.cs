// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Specification("EditableEditContent Spec")]
    class EditableEditContentSpec : FixtureSteppable
    {
        EditableEditContent<object> EditContent { get; set; } = new EditableEditContent<object>();

        bool EditCompletedRaised { get; set; }
        bool EditCanceledRaised { get; set; }

        public EditableEditContentSpec()
        {
            EditContent.EditCompleted += (s, e) => EditCompletedRaised = true;
            EditContent.EditCanceled += (s, e) => EditCanceledRaised = true;
        }

        [Example("When an edit is completed")]
        void Ex01()
        {
            When("the edit is completed", () => EditContent.CompleteEdit());
            Then("the EditCompleted event should be raised", () => EditCompletedRaised);
            Then("the EditCanceled event should not be raised", () => !EditCanceledRaised);
        }

        [Example("When an edit is canceled")]
        void Ex02()
        {
            When("the edit is canceled", () => EditContent.CancelEdit());
            Then("the EditCompleted event should not be raised", () => !EditCompletedRaised);
            Then("the EditCanceled event should be raised", () => EditCanceledRaised);
        }

        [Example("When a value specified to the constructor is null")]
        void Ex03()
        {
            When("the value that is null is specified to the constructor", () => EditContent = new EditableEditContent<object>(null));
            Then("the value of the edit content should be null without throwing an exception", () => EditContent.Value.Value == null);
        }
    }
}
