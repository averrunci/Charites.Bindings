// Copyright (C) 2019-2021 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Multi binding")]
    class BoundProperty_Spec_Binding_MultiBinding : FixtureSteppable
    {
       BoundProperty<string> Property { get; }

        ObservableProperty<int> Property1 { get; set; }
        ObservableProperty<int> Property2 { get; set; }
        ObservableProperty<int> Property3 { get; set; }

        ObservableProperty<int> Property4 { get; set; }
        ObservableProperty<string> Property5 { get; set; }
        ObservableProperty<bool> Property6 { get; set; }

        BoundProperty<int> BoundProperty1 { get; set; }
        BoundProperty<int> BoundProperty2 { get; set; }
        BoundProperty<int> BoundProperty3 { get; set; }

        [Background("the property whose value is string is ready")]
        public BoundProperty_Spec_Binding_MultiBinding()
        {
            Property = BoundProperty<string>.Of("Test1");
        }

        [Example("When the property binds some properties and the value of the property is changed")]
        void Ex01()
        {
            Given("a property whose value type is int", () => Property1 = ObservableProperty<int>.Of(1));
            Given("a property whose value type is int", () => Property2 = ObservableProperty<int>.Of(2));
            Given("a property whose value type is int", () => Property3 = ObservableProperty<int>.Of(3));
            When("the property binds the given three properties with a converter that converts to the sum of these values", () =>
                Property.Bind(context => (context.GetValueAt<int>(0) + context.GetValueAt<int>(1) + context.GetValueAt<int>(2)).ToString(), Property1, Property2, Property3)
            );
            Then("the value of the property should be the changed value", () => Property.Value == "6");
            Then("the value of the first property should not be changed", () => Property1.Value == 1);
            Then("the value of the second property should not be changed", () => Property2.Value == 2);
            Then("the value of the third property should not be changed", () => Property3.Value == 3);

            When("the value of the first property is changed", () => Property1.Value = 7);
            Then("the value of the property should be the changed value that is converted", () => Property.Value == "12");
            Then("the value of the first property should be the changed value", () => Property1.Value == 7);
            Then("the value of the second property should not be changed", () => Property2.Value == 2);
            Then("the value of the third property should not be changed", () => Property3.Value == 3);

            When("the value of the second property is changed", () => Property2.Value = 8);
            Then("the value of the property should be the changed value that is converted", () => Property.Value == "18");
            Then("the value of the first property should not be changed", () => Property1.Value == 7);
            Then("the value of the second property should be the changed value", () => Property2.Value == 8);
            Then("the value of the third property should not be changed", () => Property3.Value == 3);

            When("the value of the third property is changed", () => Property3.Value = 9);
            Then("the value of the property should be the changed value that is converted", () => Property.Value == "24");
            Then("the value of the first property should not be changed", () => Property1.Value == 7);
            Then("the value of the second property should not be changed", () => Property2.Value == 8);
            Then("the value of the third property should be the changed value", () => Property3.Value == 9);
        }

        [Example("When the property unbinds and the value of the property is changed")]
        void Ex02()
        {
            Given("a property whose value type is int", () => Property1 = ObservableProperty<int>.Of(1));
            Given("a property whose value type is int", () => Property2 = ObservableProperty<int>.Of(2));
            Given("a property whose value type is int", () => Property3 = ObservableProperty<int>.Of(3));
            When("the property binds the given three properties with a converter that converts to the sum of these values", () =>
                Property.Bind(context => (context.GetValueAt<int>(0) + context.GetValueAt<int>(1) + context.GetValueAt<int>(2)).ToString(), Property1, Property2, Property3)
            );
            Then("the value of the property should be the changed value", () => Property.Value == "6");
            Then("the value of the first property should not be changed", () => Property1.Value == 1);
            Then("the value of the second property should not be changed", () => Property2.Value == 2);
            Then("the value of the third property should not be changed", () => Property3.Value == 3);

            When("the property unbinds", () => Property.Unbind());
            When("the value of the first property is changed", () => Property1.Value = 7);
            Then("the value of the property should not be changed", () => Property.Value == "6");
            Then("the value of the first property should be the changed value", () => Property1.Value == 7);
            Then("the value of the second property should not be changed", () => Property2.Value == 2);
            Then("the value of the third property should not be changed", () => Property3.Value == 3);

            When("the value of the second property is changed", () => Property2.Value = 8);
            Then("the value of the property should not be changed", () => Property.Value == "6");
            Then("the value of the first property should not be changed", () => Property1.Value == 7);
            Then("the value of the second property should be the changed value", () => Property2.Value == 8);
            Then("the value of the third property should not be changed", () => Property3.Value == 3);

            When("the value of the third property is changed", () => Property3.Value = 9);
            Then("the value of the property should not be changed", () => Property.Value == "6");
            Then("the value of the first property should not be changed", () => Property1.Value == 7);
            Then("the value of the second property should not be changed", () => Property2.Value == 8);
            Then("the value of the third property should be the changed value", () => Property3.Value == 9);
        }

        [Example("When the property binds some properties that have the different value type and the value of the property is changed")]
        void Ex03()
        {
            Given("a property whose value type is int", () => Property4 = ObservableProperty<int>.Of(1));
            Given("a property whose value type is string", () => Property5 = ObservableProperty<string>.Of("#"));
            Given("a property whose value type is bool", () => Property6 = ObservableProperty<bool>.Of(false));
            When("the property binds the given three properties with a converter", () =>
                Property.Bind(
                    context => context.GetValueAt<bool>(2) ? $"[{context.GetValueAt<string>(1)}{context.GetValueAt<int>(0)}]" : $"{context.GetValueAt<string>(1)}{context.GetValueAt<int>(0)}",
                    Property4, Property5, Property6
                )
            );
            Then("the value of the property should be the converted value", () => Property.Value == "#1");
            Then("the value of the first property should not be changed", () => Property4.Value == 1);
            Then("the value of the second property should not be changed", () => Property5.Value == "#");
            Then("the value of the third property should not be changed", () => Property6.Value == false);

            When("the value of the first property is changed", () => Property4.Value = 7);
            Then("the value of the property should be the changed value that is converted", () => Property.Value == "#7");
            Then("the value of the first property should be the changed value", () => Property4.Value == 7);
            Then("the value of the second property should not be changed", () => Property5.Value == "#");
            Then("the value of the third property should not be changed", () => Property6.Value == false);

            When("the value of the second property is changed", () => Property5.Value = "## ");
            Then("the value of the property should be the changed value that is converted", () => Property.Value == "## 7");
            Then("the value of the first property should not be changed", () => Property4.Value == 7);
            Then("the value of the second property should be the changed value", () => Property5.Value == "## ");
            Then("the value of the third property should not be changed", () => Property6.Value == false);

            When("the value of the third property is changed", () => Property6.Value = true);
            Then("the value of the property should be the changed value that is converted", () => Property.Value == "[## 7]");
            Then("the value of the first property should not be changed", () => Property4.Value == 7);
            Then("the value of the second property should not be changed", () => Property5.Value == "## ");
            Then("the value of the third property should be the changed value", () => Property6.Value);
        }

        [Example("When the property that has already bound another property binds some properties")]
        void Ex04()
        {
            Given("a property whose value type is int", () => Property1 = ObservableProperty<int>.Of(1));
            Given("a property whose value type is int", () => Property2 = ObservableProperty<int>.Of(2));
            Given("a property whose value type is int", () => Property3 = ObservableProperty<int>.Of(3));
            When("the property binds the given three properties with a converter that converts to the sum of these values", () =>
                Property.Bind(context => (context.GetValueAt<int>(0) + context.GetValueAt<int>(1) + context.GetValueAt<int>(2)).ToString(), Property1, Property2, Property3)
            );
            When("the property binds another properties with a converter", () =>
                Property.Bind(
                    context => (context.GetValueAt<int>(0) + context.GetValueAt<int>(1) + context.GetValueAt<int>(2)).ToString(),
                    ObservableProperty<int>.Of(4), ObservableProperty<int>.Of(5), ObservableProperty<int>.Of(6)
                )
            );
            Then<InvalidOperationException>($"{typeof(InvalidOperationException)} should be thrown");
        }

        [Example("When the specified converter is null")]
        void Ex05()
        {
            Given("a property whose value type is int", () => Property1 = ObservableProperty<int>.Of(1));
            Given("a property whose value type is int", () => Property2 = ObservableProperty<int>.Of(2));
            Given("a property whose value type is int", () => Property3 = ObservableProperty<int>.Of(3));
            When("the property binds the given three properties with a converter that is null", () => Property.Bind(null, Property1, Property2, Property3));
            Then<ArgumentNullException>($"{typeof(ArgumentNullException)} should be thrown");
        }

        [Example("When the specified bound property is null")]
        void Ex06()
        {
            When("the property binds the property that is null with a converter", () => Property.Bind(context => context.GetValueAt<int>(0).ToString(), null));
            Then<ArgumentNullException>($"{typeof(ArgumentNullException)} should be thrown");
        }

        [Example("When the specified property is BoundProperty")]
        void Ex07()
        {
            Given("a property whose type is BoundProperty", () => BoundProperty1 = BoundProperty<int>.Of(1));
            Given("a property whose type is BoundProperty", () => BoundProperty2 = BoundProperty<int>.Of(2));
            Given("a property whose type is BoundProperty", () => BoundProperty3 = BoundProperty<int>.Of(3));
            When("the property binds the given three properties with a converter that converts to the sum of these values", () =>
                Property.Bind(context => (context.GetValueAt<int>(0) + context.GetValueAt<int>(1) + context.GetValueAt<int>(2)).ToString(), BoundProperty1, BoundProperty2, BoundProperty3)
            );
            Then("the value of the property should be the changed value", () => Property.Value == "6");
            Then("the value of the first property should not be changed", () => BoundProperty1.Value == 1);
            Then("the value of the second property should not be changed", () => BoundProperty2.Value == 2);
            Then("the value of the third property should not be changed", () => BoundProperty3.Value == 3);
        }
    }
}
