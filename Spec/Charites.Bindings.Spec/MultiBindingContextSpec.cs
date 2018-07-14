// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Specification("MultiBindingContext Spec")]
    class MultiBindingContextSpec
    {
        [Context]
        MultiBindingContextSpec_InstanceCreation InstanceCreation { get; set; }

        [Context]
        MultiBindingContextSpec_GetValue GetValue { get; set; }
    }
}
