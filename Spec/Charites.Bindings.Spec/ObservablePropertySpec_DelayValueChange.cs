// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.Threading.Tasks;
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Delay a value change")]
    class ObservablePropertySpec_DelayValueChange : FixtureSteppable
    {
        ObservableProperty<string> Property { get; } = new ObservableProperty<string>("Test");

        [Example("When to enable and disable a delay of a value change")]
        void Ex01()
        {
            When("the delay of the property value change is enabled", () => Property.EnableDelayValueChange(TimeSpan.FromMilliseconds(100)));
            When("the property value is changed", () => Property.Value = "Changed");
            Then("the property value should not be changed", () => Property.Value == "Test");
            When("to wait for the delay time", async () => await Task.Delay(200));
            Then("the property value should be changed", () => Property.Value == "Changed");

            When("the delay of the property value change is disabled", () => Property.DisableDelayValueChange());
            When("the property value is changed", () => Property.Value = "Modified");
            Then("the property value should be changed", () => Property.Value == "Modified");
        }
    }
}
