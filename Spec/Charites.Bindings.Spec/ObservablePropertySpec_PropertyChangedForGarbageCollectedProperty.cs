// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.ComponentModel;
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("PropertyChanged event for a garbage collected property")]
    class ObservablePropertySpec_PropertyChangedForGarbageCollectedProperty : FixtureSteppable
    {
        PropertyChangedEventHandler PropertyChangedHandler { get; }
        ObservableProperty<string> SourceProperty { get; } = ObservableProperty<string>.Of("Test");

        bool PropertyChangedHandled { get; set; }

        public ObservablePropertySpec_PropertyChangedForGarbageCollectedProperty()
        {
            PropertyChangedHandler = OnPropertyChanged;
        }

        class TestElement
        {
            private readonly PropertyChangedEventHandler propertyChangedHandler;

            public TestElement(ObservableProperty<string> property, PropertyChangedEventHandler handler)
            {
                property.PropertyChanged += OnPropertyChanged;
                propertyChangedHandler = handler;
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => propertyChangedHandler(sender, e);
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChangedHandled = e.PropertyName == "Value";

        void ExecuteGarbageCollection()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        [Example("When an element subscribes the PropertyChanged event")]
        void Ex01()
        {
            Given("an element that handles the PropertyChanged event of the source property", () => new TestElement(SourceProperty, OnPropertyChanged));
            When("the element is collected as a garbage", () => ExecuteGarbageCollection());
            When("the value of the source property is changed", () => SourceProperty.Value = "Changed");
            Then("the element that is collected as a garbage should not handle the PropertyChanged event", () => !PropertyChangedHandled);
        }

        [Example("When another property is bound")]
        void Ex02()
        {
            ObservableProperty<string> property;
            Given("a observable property that binds the source property", () =>
            {
                property = new ObservableProperty<string>();
                property.Bind(SourceProperty);
                property.PropertyChanged += PropertyChangedHandler;
            });
            When("the property is collected as a garbage", () =>
            {
                property = null;
                ExecuteGarbageCollection();
            });
            When("the value of the source property is changed", () => SourceProperty.Value = "Changed");
            Then("the property that is collected as a garbage should not handle the PropertyChanged event", () => ! PropertyChangedHandled);
        }
    }
}
