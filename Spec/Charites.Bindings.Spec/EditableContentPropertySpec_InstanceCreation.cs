// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Instance creation")]
    class EditableContentPropertySpec_InstanceCreation : FixtureSteppable
    {
        EditableContentProperty<object> Property { get; set; }

        EditableDisplayContent<object> DisplayContent { get; } = new EditableDisplayContent<object>();
        EditableEditContent<object> EditContent { get; } = new EditableEditContent<object>();

        [Example("Whe the specified display content and edit content are not null")]
        void Ex01()
        {
            When("the display content that is not null and the edit content that is not null is specified to the constructor", () => Property = new EditableContentProperty<object>(DisplayContent, EditContent));
            Then("the content of the editable content property should be the display content", () => Property.Content.Value == DisplayContent);
        }

        [Example("When the specified display content is null")]
        void Ex02()
        {
            When("the display content that is null and the edit content that is not null is specified to the constructor", () => Property = new EditableContentProperty<object>(null, EditContent));
            Then<ArgumentNullException>($"{typeof(ArgumentNullException)} should be thrown");
        }

        [Example("When the specified edit content is null")]
        void Ex03()
        {
            When("the display content that is not null and the edit content that is null is specified to the constructor", () => Property = new EditableContentProperty<object>(DisplayContent, null));
            Then<ArgumentNullException>($"{typeof(ArgumentNullException)} should be thrown");
        }
    }
}
