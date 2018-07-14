// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System.ComponentModel;
using Carna;
using NSubstitute;

namespace Charites.Windows.Mvc.Bindings
{
    [Context("Get value")]
    class MultiBindingContextSpec_GetValue : FixtureSteppable
    {
        MultiBindingContext Context { get; } = new MultiBindingContext(new[]
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
        void Ex04() =>  Expect("the default value should be got", () => Context.GetValueAt<string>(-1) == default(string));

        [Example("When the specified index is the upper range")]
        void Ex05() => Expect("the default value should be got", () => Context.GetValueAt<int>(4) == default(int));

        [Example("When the type of the specified index is not ObservableProperty")]
        void Ex06() => Expect("the default value should be got", () => Context.GetValueAt<bool>(2) == default(bool));

        [Example("When the type of the value of the specified index is not different")]
        void Ex07() => Expect("the default value should be got", () => Context.GetValueAt<string>(1) == default(string));
    }
}
