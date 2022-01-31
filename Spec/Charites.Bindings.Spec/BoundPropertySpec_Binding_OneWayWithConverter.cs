// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("One way with a converter")]
class BoundPropertySpec_Binding_OneWayWithConverter : FixtureSteppable
{
    BoundProperty<string> Property1 { get; }
    ObservableProperty<int> Property2 { get; }

    [Background("two properties (property1 that is the BoundProperty and property2) are ready; one has the string value and the other has the int value")]
    public BoundPropertySpec_Binding_OneWayWithConverter()
    {
        Property1 = BoundProperty<string>.Of("Test1");
        Property2 = ObservableProperty<int>.Of(3);
    }

    [Example("When the property binds another property with a converter and the value of the property is changed")]
    void Ex01()
    {
        When("the property1 binds the property2 with a converter", () => Property1.Bind(Property2, value => value.ToString()));
        Then("the value of the property1 should be the converted value of the property2", () => Property1.Value == "3");
        Then("the value of the property2 should not be changed", () => Property2.Value == 3);

        When("the value of the property2 is changed", () => Property2.Value = 7);
        Then("the value of the property1 should be the changed value that is converted", () => Property1.Value == "7");
        Then("the value of the property2 should be the changed value", () => Property2.Value == 7);
    }

    [Example("When the property unbinds and the value of the property is changed")]
    void Ex02()
    {
        When("the property1 binds the property2 with a converter", () => Property1.Bind(Property2, value => value.ToString()));
        Then("the value of the property1 should be the converted value of the property2", () => Property1.Value == "3");
        Then("the value of the property2 should not be changed", () => Property2.Value == 3);

        When("the property1 unbinds", () => Property1.Unbind());
        When("the value of the property2 is changed", () => Property2.Value = 7);
        Then("the value of the property1 should not be changed", () => Property1.Value == "3");
        Then("the value of the property2 should be the changed value", () => Property2.Value == 7);
    }

    [Example("When the property that has already bound another property binds another property with a converter")]
    void Ex03()
    {
        When("the property1 binds the property2 with a converter", () => Property1.Bind(Property2, value => value.ToString()));
        When("the property1 binds another property with a converter", () => Property1.Bind(ObservableProperty<int>.Of(7), value => value.ToString()));
        Then<InvalidOperationException>($"{typeof(InvalidOperationException)} should be thrown");
    }
}