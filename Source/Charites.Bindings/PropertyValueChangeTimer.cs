// Copyright (C) 2022 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
namespace Charites.Windows.Mvc.Bindings;

internal class PropertyValueChangeTimer<T>
{
    private readonly Action<string, T, T> propertyValueChangeAction;
    private System.Timers.Timer? timer;
    private SynchronizationContext? synchronizationContext;

    private string propertyName = string.Empty;
    private Tuple<T, T>? propertyValues;

    public PropertyValueChangeTimer(Action<string, T, T> propertyValueChangeAction)
    {
        this.propertyValueChangeAction = propertyValueChangeAction;
    }

    public void Enable(TimeSpan delayTime, SynchronizationContext? synchronizationContext)
    {
        Disable();

        timer = new System.Timers.Timer
        {
            Interval = delayTime.TotalMilliseconds,
        };
        timer.Elapsed += OnTimerElapsed;
        this.synchronizationContext = synchronizationContext;
    }

    public void Disable()
    {
        timer?.Stop();
        if (timer is not null) timer.Elapsed -= OnTimerElapsed;
        timer = null;
        synchronizationContext = null;
    }

    public void Restart(string propertyName, T oldValue, T newValue)
    {
        this.propertyName = propertyName;
        propertyValues = new Tuple<T, T>(oldValue, newValue);

        if (timer is null)
        {
            InvokePropertyValueChangeAction();
            return;
        }

        timer.Stop();
        timer.Start();
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        timer?.Stop();

        if (propertyValues is null) return;

        if (synchronizationContext is null)
        {
            InvokePropertyValueChangeAction();
        }
        else
        {
            synchronizationContext.Post(_ => InvokePropertyValueChangeAction(), null);
        }
    }

    private void InvokePropertyValueChangeAction()
    {
        if (propertyValues is null) return;

        propertyValueChangeAction.Invoke(propertyName, propertyValues.Item1, propertyValues.Item2);
    }
}