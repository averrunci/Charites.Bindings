// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Binding")]
class ObservablePropertySpec_Binding
{
    [Context]
    ObservablePropertySpec_Binding_OneWay OneWay => default!;

    [Context]
    ObservablePropertySpec_Binding_OneWayWithConverter OneWayWithConverter => default!;

    [Context]
    ObservablePropertySpec_Binding_MultiBinding MultiBinding => default!;

    [Context]
    ObservablePropertySpec_Binding_TwoWay TwoWay => default!;

    [Context]
    ObservablePropertySpec_Binding_TwoWayWithConverter TwoWayWithConverter => default!;
}