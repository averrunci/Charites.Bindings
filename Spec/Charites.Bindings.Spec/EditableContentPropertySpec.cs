// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Specification("EditableContentProperty Sped")]
    class EditableContentPropertySpec
    {
        [Context]
        EditableContentPropertySpec_InstanceCreation InstanceCreation { get; set; }

        [Context]
        EditableContentPropertySpec_EditStart EditStart { get; set; }

        [Context]
        EditableContentPropertySpec_EditCompletion EditCompletion { get; set; }

        [Context]
        EditableContentPropertySpec_EditCancellation EditCancellation { get; set; }
    }
}
