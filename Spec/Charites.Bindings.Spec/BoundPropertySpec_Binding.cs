// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Binding")]
class BoundPropertySpec_Binding
{
    [Context]
    BoundPropertySpec_Binding_OneWay OneWay => default!;

    [Context]
    BoundPropertySpec_Binding_OneWayWithConverter OneWayWithConverter => default!;

    [Context]
    BoundPropertySpec_Binding_MultiBinding MultiBinding => default!;
}