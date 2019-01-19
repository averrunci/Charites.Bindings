// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Specification("ObservableProperty Spec")]
    class ObservablePropertySpec
    {
        [Context]
        ObservablePropertySpec_InstanceCreation InstanceCreation { get; set; }

        [Context]
        ObservablePropertySpec_PropertyChanged PropertyChanged { get; set; }

        [Context]
        ObservablePropertySpec_Binding Binding { get; set; }

        [Context]
        ObservablePropertySpec_ValueChangeHandling ValueChangeHandling { get; set; }

        [Context]
        ObservablePropertySpec_Validation Validation { get; set; }

        [Context]
        ObservablePropertySpec_DelayValueChange DelayValueChange { get; set; }
    }
}
