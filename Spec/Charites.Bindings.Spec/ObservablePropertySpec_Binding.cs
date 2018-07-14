// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Binding")]
    class ObservablePropertySpec_Binding
    {
        [Context]
        ObservablePropertySpec_Binding_OneWay OneWay { get; set; }

        [Context]
        ObservablePropertySpec_Binding_OneWayWithConverter OneWayWithConverter { get; set; }

        [Context]
        ObservablePropertySpec_Binding_MultiBinding MultiBinding { get; set; }

        [Context]
        ObservablePropertySpec_Binding_TwoWay TwoWay { get; set; }

        [Context]
        ObservablePropertySpec_Binding_TwoWayWithConverter TwoWayWithConverter { get; set; }
    }
}
