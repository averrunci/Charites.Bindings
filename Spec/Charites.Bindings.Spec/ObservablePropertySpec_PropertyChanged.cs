// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Property changed")]
class ObservablePropertySpec_PropertyChanged : FixtureSteppable
{
    ObservableProperty<string?> Property { get; set; } = default!;
    bool PropertyChangedRaised { get; set; }

    void CreateObservablePropertyOf(string? value, bool successPropertyChangedRaisedAtAnyPropertyNames = false)
    {
        Property = ObservableProperty<string?>.Of(value);
        Property.PropertyChanged += (_, e) => PropertyChangedRaised = successPropertyChangedRaisedAtAnyPropertyNames || e.PropertyName == "Value";
    }

    [Example("When the value that is not null is set to the property whose initial value is not null")]
    void Ex01()
    {
        Given("a property whose value is not null", () => CreateObservablePropertyOf("Test"));
        When("the value that is not null is set to the property", () => Property.Value = "Changed");
        Then("the PropertyChanged event should be raised", () => PropertyChangedRaised);
    }

    [Example("When the value that is null is set to the property whose initial value is not null")]
    void Ex02()
    {
        Given("a property whose value is not null", () => CreateObservablePropertyOf("Test"));
        When("the value that is null is set to the property", () => Property.Value = null);
        Then("the PropertyChanged event should be raised", () => PropertyChangedRaised);
    }

    [Example("When the value that is the same as the value of the property is set to the property whose initial value is not null")]
    void Ex03()
    {
        Given("a property whose value is not null", () => CreateObservablePropertyOf("Test", true));
        When("the value that is the same as the value of the property is set to the property", () => Property.Value = "Test");
        Then("the PropertyChanged event should not be raised", () => !PropertyChangedRaised);
    }

    [Example("When the value that is the same as the value of the property is set to the property whose initial value is null")]
    void Ex04()
    {
        Given("a property whose value is null", () => CreateObservablePropertyOf(null, true));
        When("the value that is the same as the value of the property is set to the property", () => Property.Value = null);
        Then("the PropertyChanged event should not be raised", () => !PropertyChangedRaised);
    }

    [Example("When the value that is not null is set to the property whose initial value is null")]
    void Ex05()
    {
        Given("a property whose value is null", () => CreateObservablePropertyOf(null));
        When("the value that is not null is set to the property", () => Property.Value = "Changed");
        Then("the PropertyChanged event should be raised", () => PropertyChangedRaised);
    }
}