// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Edit completion")]
class EditableContentPropertySpec_EditCompletion : EditableContentPropertySpecContext
{
    [Example("Completes the edit after the edit is started")]
    void Ex01()
    {
        When("the edit is started", () => DisplayContent.StartEdit());
        AssertEditing(true);

        When("the edit is completed", () => EditContent.CompleteEdit());
        AssertEditing(false);

        AssertEventRaised(true, true, false);
    }

    [Example("Completes the edit before the edit is started")]
    void Ex02()
    { 
        When("the edit is completed", () => EditContent.CompleteEdit());
        AssertEditing(false);

        AssertEventRaised(false, false, false);
    }
}