// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

/// <summary>
/// Represents a scope in which the specified indicator is set to true until this instance is disposed.
/// </summary>
public sealed class ProgressScope : IDisposable
{
    private readonly ObservableProperty<bool> progressIndicator;

    private ProgressScope(ObservableProperty<bool> progressIndicator)
    {
        this.progressIndicator = progressIndicator;
        this.progressIndicator.Value = true;
    }

    /// <summary>
    /// Creates a scope in which the specified indicator is set to true until this instance is disposed.
    /// </summary>
    /// <param name="progressIndicator">The indicator that is set to true in a scope.</param>
    /// <returns>A new instance of the <see cref="ProgressScope"/> class.</returns>
    public static ProgressScope By(ObservableProperty<bool> progressIndicator) => new(progressIndicator);

    /// <summary>
    /// Sets the indicator to false.
    /// </summary>
    public void Dispose()
    {
        progressIndicator.Value = false;
    }
}