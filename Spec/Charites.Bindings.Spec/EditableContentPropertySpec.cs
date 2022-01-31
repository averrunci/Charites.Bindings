// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification("EditableContentProperty Sped")]
class EditableContentPropertySpec
{
    [Context]
    EditableContentPropertySpec_InstanceCreation InstanceCreation => default!;

    [Context]
    EditableContentPropertySpec_EditStart EditStart => default!;

    [Context]
    EditableContentPropertySpec_EditCompletion EditCompletion => default!;

    [Context]
    EditableContentPropertySpec_EditCancellation EditCancellation => default!;
}