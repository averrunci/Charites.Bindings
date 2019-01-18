// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Carna;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Validation")]
    class BoundPropertySpec_Validation : FixtureSteppable
    {
        BoundProperty<string> PropertyNotAttributedValidation { get; }

        [Display(Name = "String Expression")]
        [StringLength(10, ErrorMessage = "Please enter {0} within {1} characters.")]
        BoundProperty<string> PropertyAttributedValidation { get; }

        [StringLength(6, ErrorMessage = "Please enter within {1} characters.")]
        [RegularExpression("\\d+", ErrorMessage = "Please enter digits only.")]
        BoundProperty<string> PropertyAttributedMultiValidations { get; }

        [Required(ErrorMessage = "Please enter a value.")]
        BoundProperty<string> RequiredProperty { get; }

        [Display(Name = nameof(Resources.LocalizablePropertyName), ResourceType = typeof(Resources))]
        [StringLength(10, ErrorMessageResourceName = nameof(Resources.StringLengthErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        BoundProperty<string> LocalizablePropertyAttributedValidation { get; }

        PropertyValueValidateEventHandler<string> PropertyValueValidateHandler { get; } = (s, e) =>
        {
            if (e.Value == "Correct") return;

            e.Add("The value does not correct.");
        };

        bool ErrorsChangedRaised { get; set; }
        EventHandler<DataErrorsChangedEventArgs> ErrorsChangedVerificationHandler { get; }

        ObservableProperty<string> Property { get; } = new ObservableProperty<string>();

        public BoundPropertySpec_Validation()
        {
            PropertyNotAttributedValidation = new BoundProperty<string>().Bind(Property);
            PropertyAttributedValidation = new BoundProperty<string>().Bind(Property);
            PropertyAttributedMultiValidations = new BoundProperty<string>().Bind(Property);
            RequiredProperty = new BoundProperty<string>().Bind(Property);
            LocalizablePropertyAttributedValidation = new BoundProperty<string>().Bind(Property);

            ErrorsChangedVerificationHandler = (s, e) => ErrorsChangedRaised = true;
        }

        void SetValue<T>(T value, ObservableProperty<T> property, BoundProperty<T> boundProperty)
        {
            ErrorsChangedRaised = false;
            boundProperty.ErrorsChanged += ErrorsChangedVerificationHandler;
            property.Value = value;
            boundProperty.ErrorsChanged -= ErrorsChangedVerificationHandler;
        }

        void AssertNoValidationError<T>(BoundProperty<T> property)
        {
            Then("the Error property should be empty", () => property.Error == string.Empty);
            Then("the value of 'Value' of the indexer should be empty", () => ((IDataErrorInfo)property)["Value"] == string.Empty);
            Then("the Errors property should be empty", () => !property.Errors.Any());
            Then("the HasErrors property should be false", () => !property.HasErrors);
            Then("the value of the GetErrors whose parameter is 'Value' should be empty", () => !((INotifyDataErrorInfo)property).GetErrors("Value").Cast<string>().Any());
        }

        void AssertValidationError<T>(BoundProperty<T> property, params string[] messages)
        {
            var expectedMessage = string.Join(Environment.NewLine, messages);

            Then("the Error property should be the specified message", () => property.Error == expectedMessage);
            Then("the value of 'Value' of the indexer should be the specified message", () => ((IDataErrorInfo)property)["Value"] == expectedMessage);
            Then("the Errors property should be the specified message", () => property.Errors.SequenceEqual(messages));
            Then("the HasErrors property should be true", () => property.HasErrors);
            Then("the value of the GetErrors whose parameter is 'Value' should be the specified message", () =>
                ((INotifyDataErrorInfo)property).GetErrors("Value").Cast<string>().SequenceEqual(messages)
            );
        }

        [Example("When ValidationAttribute is not specified")]
        void Ex01()
        {
            When("the property validation is enabled", () => PropertyNotAttributedValidation.EnableValidation(() => PropertyNotAttributedValidation));
            When("the value is set", () => SetValue("Changed", Property, PropertyNotAttributedValidation));
            Then("the ErrorsChanged event should not be raised", () => !ErrorsChangedRaised);
            AssertNoValidationError(PropertyNotAttributedValidation);
        }

        [Example("When ValidationAttribute is specified and the invalid value is set to the property")]
        void Ex02()
        {
            When("the property validation is enabled", () => PropertyAttributedValidation.EnableValidation(() => PropertyAttributedValidation));
            When("the invalid value is set", () => SetValue("long values", Property, PropertyAttributedValidation));
            Then("the ErrorChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(PropertyAttributedValidation, "Please enter String Expression within 10 characters.");
        }

        [UICulture("en-US")]
        [Example("When ValidationAttribute that is localizable is specified and the invalid value is set to the property (culture is en-US)")]
        void Ex03()
        {
            When("the property validation is enabled", () => LocalizablePropertyAttributedValidation.EnableValidation(() => LocalizablePropertyAttributedValidation));
            When("the invalid value is set in en-US culture", () => SetValue("long values", Property, LocalizablePropertyAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(LocalizablePropertyAttributedValidation, "Please enter Localizable Property within 10 characters.");
        }

        [UICulture("ja-JP")]
        [Example("When ValidationAttribute that is localizable is specified and the invalid value is set to the property (culture is ja-JP)")]
        void Ex04()
        {
            When("the property validation is enabled", () => LocalizablePropertyAttributedValidation.EnableValidation(() => LocalizablePropertyAttributedValidation));
            When("the invalid value is set in ja-JP culture", () => SetValue("規定の長さよりも長い値", Property, LocalizablePropertyAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(LocalizablePropertyAttributedValidation, "ローカライズ可能なプロパティは10文字以内で入力してください。");
        }

        [Example("When ValidationAttribute is specified and the valid value is set to the property")]
        void Ex05()
        {
            When("the property validation is enabled", () => PropertyAttributedValidation.EnableValidation(() => PropertyAttributedValidation));
            When("the valid value is set", () => SetValue("long value", Property, PropertyAttributedValidation));
            Then("the ErrorsChanged event should not be raised", () => !ErrorsChangedRaised);
            AssertNoValidationError(PropertyAttributedValidation);

            When("the invalid value is set", () => SetValue("long values", Property, PropertyAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(PropertyAttributedValidation, "Please enter String Expression within 10 characters.");

            When("the valid value is set", () => SetValue("long value", Property, PropertyAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertNoValidationError(PropertyAttributedValidation);
        }

        [Example("When some ValidationAttribute are specified and the invalid value is set to the property")]
        void Ex06()
        {
            When("the property validation is enabled", () => PropertyAttributedMultiValidations.EnableValidation(() => PropertyAttributedMultiValidations));
            When("the invalid value is set", () => SetValue("invalid", Property, PropertyAttributedMultiValidations));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(PropertyAttributedMultiValidations, "Please enter within 6 characters.", "Please enter digits only.");
        }

        [Example("When som ValidationAttribute are specified and the valid value is set to the property")]
        void Ex07()
        {
            When("the property validation is enabled", () => PropertyAttributedMultiValidations.EnableValidation(() => PropertyAttributedMultiValidations));
            When("the valid value is set", () => SetValue("123456", Property, PropertyAttributedMultiValidations));
            Then("the ErrorsChanged event should not be raised", () => !ErrorsChangedRaised);
            AssertNoValidationError(PropertyAttributedMultiValidations);

            When("the invalid value is set", () => SetValue("invalid", Property, PropertyAttributedMultiValidations));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(PropertyAttributedMultiValidations, "Please enter within 6 characters.", "Please enter digits only.");

            When("the valid value is set", () => SetValue("123456", Property, PropertyAttributedMultiValidations));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertNoValidationError(PropertyAttributedMultiValidations);
        }

        [Example("When PropertyValueValidate event handler is added and the invalid value is set to the property")]
        void Ex08()
        {
            When("the property validation is enabled", () => PropertyNotAttributedValidation.EnableValidation(() => PropertyNotAttributedValidation));
            When("the PropertyValueValidate event handler is added", () => PropertyNotAttributedValidation.PropertyValueValidate += PropertyValueValidateHandler);
            When("the invalid value is set", () => SetValue("Incorrect", Property, PropertyNotAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(PropertyNotAttributedValidation, "The value does not correct.");
        }

        [Example("When PropertyValueValidate event handler is added and the valid value is set to the property")]
        void Ex09()
        {
            When("the property validation is enabled", () => PropertyNotAttributedValidation.EnableValidation(() => PropertyNotAttributedValidation));
            When("the PropertyValueValidate event handler is added", () => PropertyNotAttributedValidation.PropertyValueValidate += PropertyValueValidateHandler);
            When("the valid value is set", () => SetValue("Correct", Property, PropertyNotAttributedValidation));
            Then("the ErrorsChanged event should not be raised", () => !ErrorsChangedRaised);
            AssertNoValidationError(PropertyNotAttributedValidation);

            When("the invalid value is set", () => SetValue("Incorrect", Property, PropertyNotAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(PropertyNotAttributedValidation, "The value does not correct.");

            When("the valid value is set", () => SetValue("Correct", Property, PropertyNotAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertNoValidationError(PropertyNotAttributedValidation);
        }

        [Example("When ValidationAttribute is specified, PropertyValueValidate event handler is added, and the invalid value is set to the property")]
        void Ex10()
        {
            When("the property validation is enabled", () => PropertyAttributedValidation.EnableValidation(() => PropertyAttributedValidation));
            When("the PropertyValueValidate event handler is added", () => PropertyAttributedValidation.PropertyValueValidate += PropertyValueValidateHandler);
            When("the invalid value is set", () => SetValue("long values", Property, PropertyAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(PropertyAttributedValidation, "Please enter String Expression within 10 characters.", "The value does not correct.");
        }

        [Example("When ValidateAttribute is specified, PropertyValueValidate event handler is added, and the valid value is set to the property")]
        void Ex11()
        {
            When("the property validation is enabled", () => PropertyAttributedValidation.EnableValidation(() => PropertyAttributedValidation));
            When("the PropertyValueValidate event handler is added", () => PropertyAttributedValidation.PropertyValueValidate += PropertyValueValidateHandler);
            When("the valid value is set", () => SetValue("Correct", Property, PropertyAttributedValidation));
            Then("the ErrorsChanged event should not be raised", () => !ErrorsChangedRaised);
            AssertNoValidationError(PropertyAttributedValidation);

            When("the invalid value is set", () => SetValue("long values", Property, PropertyAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertValidationError(PropertyAttributedValidation, "Please enter String Expression within 10 characters.", "The value does not correct.");

            When("the valid value is set", () => SetValue("Correct", Property, PropertyAttributedValidation));
            Then("the ErrorsChanged event should be raised", () => ErrorsChangedRaised);
            AssertNoValidationError(PropertyAttributedValidation);
        }

        [Example("When cancelValueChangedIfInvalid is true and the invalid value is set to the property")]
        void Ex12()
        {
            When("the property validation is enabled with cancelValueChangedIfInvalid", () => PropertyAttributedValidation.EnableValidation(() => PropertyAttributedValidation, true));
            When("the valid value is set", () => SetValue("short word", Property, PropertyAttributedValidation));
            Then("the value of the property should be the changed value", () => PropertyAttributedValidation.Value == "short word");

            When("the invalid value is set", () => SetValue("long values", Property, PropertyAttributedValidation));
            Then("the value of the property should not be changed", () => PropertyAttributedValidation.Value == "short word");
        }

        [Example("When the validation is enabled and the validation is ensured")]
        void Ex13()
        {
            When("the property validation is enabled", () => RequiredProperty.EnableValidation(() => RequiredProperty));
            When("the property validation is ensured", () => RequiredProperty.EnsureValidation());
            AssertValidationError(RequiredProperty, "Please enter a value.");
        }
    }
}
