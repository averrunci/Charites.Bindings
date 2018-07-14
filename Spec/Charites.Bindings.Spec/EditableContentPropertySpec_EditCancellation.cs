// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Edit cancellation")]
    class EditableContentPropertySpec_EditCancellation : EditableContentPropertySpecContext
    {
        [Example("Cancels the edit after the edit is started")]
        void Ex01()
        {
            When("the edit is started", () => DisplayContent.StartEdit());
            AssertEditing(true);

            When("the edit is canceled", () => EditContent.CancelEdit());
            AssertEditing(false);

            AssertEventRaised(true, false, true);
        }

        [Example("Cancels the edit before the edit is started")]
        void Ex02()
        {
            When("the edit is canceled", () => EditContent.CancelEdit());
            AssertEditing(false);

            AssertEventRaised(false, false, false);
        }

        [Example("Cancels the edit by changing the IsEditing to false during editing")]
        void Ex03()
        {
            When("the edit is started", () => DisplayContent.StartEdit());
            AssertEditing(true);

            When("the IsEditing is set to false", () => EditableContent.IsEditing.Value = false);
            AssertEditing(false);

            AssertEventRaised(true, false, true);
        }

        [Example("Cancels the edit by changing the IsEditable to false during editing")]
        void Ex04()
        {
            When("the edit is started", () => DisplayContent.StartEdit());
            AssertEditing(true);

            When("the IsEditable is set to false", () => EditableContent.IsEditable.Value = false);
            AssertEditing(false);

            AssertEventRaised(true, false, true);
        }
    }
}
