// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Instance creation")]
class BoundPropertySpec_InstanceCreation : FixtureSteppable
{
    ObservableProperty<string> Property1 { get; } = new("Test");
    ObservableProperty<int> Property2 { get; } = new(3);
    ObservableProperty<bool> Property3 { get; } = new(false);

    BoundProperty<string> BoundProperty { get; set; } = default!;

    [Example("When an initial value is specified")]
    void Ex01() => Expect("the value of the bound property should be the specified value.", () => new BoundProperty<string>("Test").Value == "Test");

    [Example("When an initial value is specified using the factory method")]
    void Ex02() => Expect("the value of the bound property should be the specified value", () => BoundProperty<string>.Of("Test").Value == "Test");

    [Example("When a bindable property is specified")]
    void Ex03()
    {
        When("to create an instance with the bindable property", () => BoundProperty = new BoundProperty<string>(Property1));
        Then("the value of the bound property should be the value of the specified bindable property", () => BoundProperty.Value == "Test");

        When("to change the value of the bindable property" , () => Property1.Value = "Changed");
        Then("the value of the bound property should be the changed value", () => BoundProperty.Value == "Changed");
    }

    [Example("When a bindable property is specified using the factory method")]
    void Ex04()
    {
        When("to create an instance with the bindable property using the factory method", () => BoundProperty = BoundProperty<string>.By(Property1));
        Then("the value of the bound property should be the value of the specified bindable property", () => BoundProperty.Value == "Test");

        When("to change the value of the bindable property", () => Property1.Value = "Changed");
        Then("the value of the bound property should be the changed value", () => BoundProperty.Value == "Changed");
    }

    [Example("When a bindable property and a converter is specified using the factory method")]
    void Ex05()
    {
        When("to create an instance with the bindable property and the converter using the factory method", () => BoundProperty = BoundProperty<string>.By(Property2, value => value.ToString()));
        Then("the value of the bound property should be the converted value of the specified bindable property", () => BoundProperty.Value == "3");

        When("to change the value of the bindable property", () => Property2.Value = 7);
        Then("the value of the bound property should be the converted changed value", () => BoundProperty.Value == "7");
    }

    [Example("When some bindable properties and a converter is specified using the factory method")]
    void Ex06()
    {
        When("to create an instance with some bindable properties and the converter is specified using the factory method", () =>
            BoundProperty = BoundProperty<string>.By(
                context => context.GetValueAt<bool>(2) ? $"[{context.GetValueAt<string>(0)}{context.GetValueAt<int>(1)}]" : $"{context.GetValueAt<string>(0)}{context.GetValueAt<int>(1)}",
                Property1, Property2, Property3
            )
        );
        Then("the value of the bound property should be the converted value of the specified bindable properties", () => BoundProperty.Value == "Test3");

        When("to change the value of the bindable property", () => Property3.Value = true);
        Then("the value of the bound property should be the converted changed value", () => BoundProperty.Value == "[Test3]");
    }
}