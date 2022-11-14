// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System.ComponentModel;
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Context("Get values")]
class MultiBindingContextSpec_GetValues : FixtureSteppable
{
    [Example("When all value types in the binding source are the same")]
    void Ex01()
    {
        var context = new MultiBindingContext(new[]
        {
            ObservableProperty<int>.Of(1),
            ObservableProperty<int>.Of(2),
            ObservableProperty<int>.Of(3)
        });
        Expect("all values should be got", () => context.GetValues<int>().SequenceEqual(new[] { 1, 2, 3 }));
    }

    [Example("When some value types in the binding source are different")]
    void Ex02()
    {
        var context = new MultiBindingContext(new INotifyPropertyChanged[]
        {
            ObservableProperty<int>.Of(1),
            ObservableProperty<bool>.Of(true),
            ObservableProperty<int>.Of(3)
        });
        Expect("the values whose types are the same as the specified type should be got", () => context.GetValues<int>().SequenceEqual(new[] { 1, 3 }));
    }

    [Example("When all value types in the binding source are different")]
    void Ex03()
    {
        var context = new MultiBindingContext(new[]
        {
            ObservableProperty<int>.Of(1),
            ObservableProperty<int>.Of(2),
            ObservableProperty<int>.Of(3)
        });
        Expect("an empty enumerable should be got", () => !context.GetValues<string>().Any());
    }
}