// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Instance Creation")]
    class MultiBindingContextSpec_InstanceCreation : FixtureSteppable
    {
        [Example("When the specified enumerable of sources is null")]
        void Ex01()
        {
            When("the instance is created with the specified enumerable of sources that is null", () => new MultiBindingContext(null));
            Then<ArgumentNullException>($"{typeof(ArgumentNullException)} should be thrown");
        }
    }
}
