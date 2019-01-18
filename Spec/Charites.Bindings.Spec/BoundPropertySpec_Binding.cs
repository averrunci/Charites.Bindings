// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Binding")]
    class BoundPropertySpec_Binding
    {
        [Context]
        BoundPropertySpec_Binding_OneWay OneWay { get; }

        [Context]
        BoundPropertySpec_Binding_OneWayWithConverter OneWayWithConverter { get; }

        [Context]
        BoundProperty_Spec_Binding_MultiBinding MultiBinding { get; }
    }
}
