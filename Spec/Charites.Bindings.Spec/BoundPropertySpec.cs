// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Specification("BoundProperty Spec")]
    public class BoundPropertySpec
    {
        [Context]
        BoundPropertySpec_InstanceCreation InstanceCreation { get; }

        [Context]
        BoundPropertySpec_Binding Binding { get; }

        [Context]
        BoundPropertySpec_ValueChangeHandling ValueChangeHandling { get; }

        [Context]
        BoundPropertySpec_Validation Validation { get; }
    }
}
