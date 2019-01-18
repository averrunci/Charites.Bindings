// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Value changing")]
    class BoundPropertySpec_ValueChangeHandling_ValueChanging : FixtureSteppable
    {
        BoundProperty<string> BoundProperty { get; set; }
        ObservableProperty<string> Property { get; set; }

        string OldPropertyValue { get; set; }
        string NewPropertyValue { get; set; }

        [Example("When the value changing handler is added")]
        void Ex01()
        {
            Given("a property whose value is null", () => Property = new ObservableProperty<string>());
            Given("a BoundProperty that binds a given property", () => BoundProperty = BoundProperty<string>.Of(null).Bind(Property));
            When("the BoundProperty value changing handler is added", () => BoundProperty.PropertyValueChanging += (s, e) =>
            {
                OldPropertyValue = e.OldValue;
                NewPropertyValue = e.NewValue;
            });
            When("the value of the property is changed", () => Property.Value = "Changed");
            Then("the old value of the property value changing handler should be the previous value", () => OldPropertyValue == null);
            Then("the new value of the property value changing handler should be the changed value", () => NewPropertyValue == "Changed");
            Then("the value of the BoundProperty should be the changed value", () => Property.Value == "Changed");
        }

        [Example("When the value changing is canceled")]
        void Ex02()
        {
            Given("a property whose value is not null", () => Property = ObservableProperty<string>.Of("Test"));
            Given("a BoundProperty that binds a given property", () => BoundProperty = BoundProperty<string>.Of(null).Bind(Property));
            When("the property value changing handler in which the value changing is canceled is added", () => Property.PropertyValueChanging += (s, e) =>
            {
                OldPropertyValue = e.OldValue;
                NewPropertyValue = e.NewValue;

                e.Disable();
            });
            When("the value of the property is changed", () => Property.Value = "Changed");
            Then("the old value of the property value changing handler should be the previous value", () => OldPropertyValue == "Test");
            Then("the new value of the property value changing handler should be the changed value", () => NewPropertyValue == "Changed");
            Then("the value of the BoundProperty should not be changed", () => BoundProperty.Value == "Test");
        }
    }
}
