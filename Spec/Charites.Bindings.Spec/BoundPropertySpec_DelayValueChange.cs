// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Delay a value change")]
class BoundPropertySpec_DelayValueChange : FixtureSteppable
{
    BoundProperty<string> BoundProperty { get; } = new(string.Empty);
    ObservableProperty<string> Property { get; } = new("Test");

    public BoundPropertySpec_DelayValueChange()
    {
        BoundProperty.Bind(Property);
    }

    [Example("When to enable and disable a delay of a value change")]
    void Ex01()
    {
        When("the delay of the property value change of the BoundProperty is enabled", () => BoundProperty.EnableDelayValueChange(TimeSpan.FromMilliseconds(100)));
        When("the property value is changed", () => Property.Value = "Changed");
        Then("the BoundProperty value should not be changed", () => BoundProperty.Value == "Test");
        When("to wait for the delay time", async () => await Task.Delay(200));
        Then("the BoundProperty value should be changed", () => BoundProperty.Value == "Changed");

        When("the delay of the property value change of the BoundProperty is disabled", () => BoundProperty.DisableDelayValueChange());
        When("the property value is changed", () => Property.Value = "Modified");
        Then("the BoundProperty value should be changed", () => BoundProperty.Value == "Modified");
    }
}