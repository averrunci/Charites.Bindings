// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Value change handling")]
    class BoundPropertySpec_ValueChangeHandling
    {
        [Context]
        BoundPropertySpec_ValueChangeHandling_ValueChanging ValueChanging { get; }
    }
}
