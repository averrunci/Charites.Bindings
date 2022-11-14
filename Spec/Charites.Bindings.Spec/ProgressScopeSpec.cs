// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Carna;

namespace Charites.Windows.Mvc.Bindings;

[Specification("ProgressScope Spec")]
class ProgressScopeSpec : FixtureSteppable
{
    ObservableProperty<bool> ProgressIndicator { get; } = false.ToObservableProperty();

    ProgressScope Scope { get; set; } = default!;

    [Example("Begins/ends a scope")]
    void Ex01()
    {
        Expect("the initial indicator value should be false", () => !ProgressIndicator.Value);

        When("to begin a scope by specifying the indicator", () => Scope = ProgressScope.By(ProgressIndicator));
        Then("the indicator value should be true", () => ProgressIndicator.Value);
        When("to end the scope", () => Scope.Dispose());
        Then("the indicator value should be false", () => !ProgressIndicator.Value);
    }
}