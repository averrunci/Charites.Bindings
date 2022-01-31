// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification("ObservableProperty Spec")]
class ObservablePropertySpec
{
    [Context]
    ObservablePropertySpec_InstanceCreation InstanceCreation => default!;

    [Context]
    ObservablePropertySpec_PropertyChanged PropertyChanged => default!;

    [Context]
    ObservablePropertySpec_Binding Binding => default!;

    [Context]
    ObservablePropertySpec_ValueChangeHandling ValueChangeHandling => default!;

    [Context]
    ObservablePropertySpec_Validation Validation => default!;

    [Context]
    ObservablePropertySpec_DelayValueChange DelayValueChange => default!;
}