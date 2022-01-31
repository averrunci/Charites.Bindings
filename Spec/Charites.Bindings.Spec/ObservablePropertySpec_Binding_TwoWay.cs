// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Two way")]
class ObservablePropertySpec_Binding_TwoWay : FixtureSteppable
{
    ObservableProperty<string> Property1 { get; }
    ObservableProperty<string> Property2 { get; }

    [Background("two properties (property1 and property2) are ready")]
    public ObservablePropertySpec_Binding_TwoWay()
    {
        Property1 = ObservableProperty<string>.Of("Test1");
        Property2 = ObservableProperty<string>.Of("Test2");
    }

    [Example("When the property binds another property as two way binding and the value of the property is changed")]
    void Ex01()
    {
        When("the property1 binds the property2 as two way binding", () => Property1.BindTwoWay(Property2));
        Then("the value of the property1 should be the value of the property2", () => Property1.Value == "Test2");
        Then("the value of the property2 should not be changed", () => Property2.Value == "Test2");

        When("the value of the property2 is changed", () => Property2.Value = "Changed");
        Then("the value of the property1 should be the changed value", () => Property1.Value == "Changed");
        Then("the value of the property2 should be the changed value", () => Property2.Value == "Changed");

        When("the value of the property1 is changed", () => Property1.Value = "Test");
        Then("the value of the property1 should be the changed value", () => Property1.Value == "Test");
        Then("the value of the property2 should be the changed value", () => Property2.Value == "Test");
    }

    [Example("When the property unbinds the bound property and the value of the property is changed")]
    void Ex02()
    {
        When("the property1 binds the property2 as two way binding", () => Property1.BindTwoWay(Property2));
        Then("the value of the property1 should be the value of the property2", () => Property1.Value == "Test2");
        Then("the value of the property2 should not be changed", () => Property2.Value == "Test2");

        When("the property1 unbinds the property2", () => Property1.UnbindTwoWay(Property2));
        When("the value of the property2 is changed", () => Property2.Value = "Changed");
        Then("the value of the property1 should not be changed", () => Property1.Value == "Test2");
        Then("the value of the property2 should be the changed value", () => Property2.Value == "Changed");

        When("the value of the property1 is changed", () => Property1.Value = "Test");
        Then("the value of the property1 should be the changed value", () => Property1.Value == "Test");
        Then("the value of the property2 should not be changed", () => Property2.Value == "Changed");
    }

    [Example("When the property unbinds and the value of the property is changed")]
    void Ex03()
    {
        When("the property1 binds the property2 as two way binding", () => Property1.BindTwoWay(Property2));
        Then("the value of the property1 should be the value of the property2", () => Property1.Value == "Test2");
        Then("the value of the property2 should not be changed", () => Property2.Value == "Test2");

        When("the property1 unbinds without the property2", () => Property1.Unbind());
        When("the value of the property2 is changed", () => Property2.Value = "Changed");
        Then("the value of the property1 should not be changed", () => Property1.Value == "Test2");
        Then("the value of the property2 should be the changed value", () => Property2.Value == "Changed");

        When("the value of the property1 is changed", () => Property1.Value = "Test");
        Then("the value of the property1 should be the changed value", () => Property1.Value == "Test");
        Then("the value of the property2 should be the changed value", () => Property2.Value == "Test");

        When("the property2 unbinds", () => Property2.Unbind());
        When("the value of the property2 is changed", () => Property2.Value = "Modified");
        Then("the value of the property1 should not be changed", () => Property1.Value == "Test");
        Then("the value of the property2 should be the changed value", () => Property2.Value == "Modified");

        When("the value of the property1 is changed", () => Property1.Value = "TestTest");
        Then("the value of the property1 should be the changed value", () => Property1.Value == "TestTest");
        Then("the value of the property2 should not be changed", () => Property2.Value == "Modified");
    }

    [Example("When the property that has already bound another property binds another property")]
    void Ex04()
    {
        When("the property1 binds the property2 as two way binding", () => Property1.BindTwoWay(Property2));
        When("the property1 binds another property as two way binding", () => Property1.BindTwoWay(ObservableProperty<string>.Of("Test")));
        Then<InvalidOperationException>($"{typeof(InvalidOperationException)} should be thrown");
    }

    [Example("When the property that has not bound a property yet unbinds")]
    void Ex05()
    {
        When("the property1 unbinds the property2", () => Property1.UnbindTwoWay(Property2));
        Then<InvalidOperationException>($"{typeof(InvalidOperationException)} should be thrown");
    }
}