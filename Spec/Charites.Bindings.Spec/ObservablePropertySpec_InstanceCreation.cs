// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Instance creation")]
    class ObservablePropertySpec_InstanceCreation : FixtureSteppable
    {
        [Example("When an initial value is specified")]
        void Ex01() => Expect("the value of the property should be the specified value.", () => new ObservableProperty<string>("Test").Value == "Test");

        [Example("When an initial value is specified using the factory method")]
        void Ex02() => Expect("the value of the property should be the specified value", () => ObservableProperty<string>.Of("Test").Value == "Test");
    }
}
