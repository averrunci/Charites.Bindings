// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification("BoundProperty Spec")]
public class BoundPropertySpec
{
    [Context]
    BoundPropertySpec_InstanceCreation InstanceCreation => default!;

    [Context]
    BoundPropertySpec_Binding Binding => default!;

    [Context]
    BoundPropertySpec_ValueChangeHandling ValueChangeHandling => default!;

    [Context]
    BoundPropertySpec_Validation Validation => default!;

    [Context]
    BoundPropertySpec_DelayValueChange DelayValueChange => default!;
}