// Copyright (C) 2022-2023 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification(
    "BoundProperty Spec",
    typeof(BoundPropertySpec_InstanceCreation),
    typeof(BoundPropertySpec_Binding),
    typeof(BoundPropertySpec_ValueChangeHandling),
    typeof(BoundPropertySpec_Validation),
    typeof(BoundPropertySpec_DelayValueChange)
)]
public class BoundPropertySpec;