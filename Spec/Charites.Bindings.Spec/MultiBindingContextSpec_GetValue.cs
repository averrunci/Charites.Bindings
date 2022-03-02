// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System.ComponentModel;
using Carna;
using NSubstitute;

namespace Charites.Windows.Mvc.Bindings;

[Context("Get value")]
class MultiBindingContextSpec_GetValue : FixtureSteppable
{
    MultiBindingContext Context { get; } = new(new[]
    {
        ObservableProperty<string>.Of("Test1"),
        ObservableProperty<int>.Of(7),
        Substitute.For<INotifyPropertyChanged>(),
        ObservableProperty<bool>.Of(true)
    });

    [Example("When the specified index is in range")]
    void Ex01() => Expect("the value at the specified index should be got", () => Context.GetValueAt<int>(1) == 7);

    [Example("When the specified index is the lower bound")]
    void Ex02() => Expect("the value at the specified index should be got", () => Context.GetValueAt<string>(0) == "Test1");

    [Example("When the specified index is the upper bound")]
    void Ex03() => Expect("the value at the specified index should be got", () => Context.GetValueAt<bool>(3));

    [Example("When the specified index is the lower range")]
    void Ex04()
    {
        When("the lower range index is specified", () => Context.GetValueAt<string>(-1));
        Then<ArgumentOutOfRangeException>($"{typeof(ArgumentOutOfRangeException)} should be thrown");
    }

    [Example("When the specified index is the upper range")]
    void Ex05()
    {
        When("the upper range index is specified", () => Context.GetValueAt<int>(4));
        Then<ArgumentOutOfRangeException>($"{typeof(ArgumentOutOfRangeException)} should be thrown");
    }

    [Example("When the type of the specified index is not BindableProperty")]
    void Ex06()
    {
        When("the index at which the type is not BindableProperty is specified", () => Context.GetValueAt<bool>(2));
        Then<ArgumentException>($"{typeof(ArgumentException)} should be thrown");
    }

    [Example("When the type of the value of the specified index is not different")]
    void Ex07()
    {
        When("the index at which the type of the value is not different is specified", () => Context.GetValueAt<string>(1));
        Then<ArgumentException>($"{typeof(ArgumentException)} should be thrown");
    }
}