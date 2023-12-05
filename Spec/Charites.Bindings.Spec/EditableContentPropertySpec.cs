// Copyright (C) 2022-2023 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification(
    "EditableContentProperty Spec",
    typeof(EditableContentPropertySpec_InstanceCreation),
    typeof(EditableContentPropertySpec_EditStart),
    typeof(EditableContentPropertySpec_EditCompletion),
    typeof(EditableContentPropertySpec_EditCancellation)
)]
class EditableContentPropertySpec;