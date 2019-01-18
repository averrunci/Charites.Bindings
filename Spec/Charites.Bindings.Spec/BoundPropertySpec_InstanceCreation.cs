// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Instance creation")]
    class BoundPropertySpec_InstanceCreation : FixtureSteppable
    {
        [Example("When an initial value is not specified")]
        void Ex01() => Expect("the value of the property should be the default value.", () => new BoundProperty<string>().Value == default(string));

        [Example("When an initial value is specified")]
        void Ex02() => Expect("the value of the property should be the specified value.", () => new BoundProperty<string>("Test").Value == "Test");

        [Example("When an initial value is specified using the factory method")]
        void Ex03() => Expect("the value of the property should be the specified value", () => BoundProperty<string>.Of("Test").Value == "Test");
    }
}