// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Value change handling")]
    class ObservablePropertySpec_ValueChangeHandling
    {
        [Context]
        ObservablePropertySpec_ValueChangeHandling_ValueChanging ValueChanging { get; set; }

        [Context]
        ObservablePropertySpec_ValueChangeHandling_ValueChanged ValueChanged { get; set; }
    }
}
