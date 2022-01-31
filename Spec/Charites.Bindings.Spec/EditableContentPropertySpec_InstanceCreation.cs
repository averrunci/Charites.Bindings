// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Instance creation")]
class EditableContentPropertySpec_InstanceCreation : FixtureSteppable
{
    EditableContentProperty<object?> Property { get; set; } = default!;

    EditableDisplayContent<object?> DisplayContent { get; } = new(null);
    EditableEditContent<object?> EditContent { get; } = new(null);

    [Example("Whe the specified display content and edit content are not null")]
    void Ex01()
    {
        When("the display content that is not null and the edit content that is not null is specified to the constructor", () => Property = new EditableContentProperty<object?>(DisplayContent, EditContent));
        Then("the content of the editable content property should be the display content", () => Property.Content.Value == DisplayContent);
    }
}