// Copyright (C) 2019 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.Timers;

namespace Charites.Windows.Mvc.Bindings
{
    internal class PropertyValueChangeTimer<T>
    {
        private readonly Action<string, T, T> propertyValueChangeAction;
        private Timer timer;
        private System.Threading.SynchronizationContext synchronizationContext;

        private string propertyName;
        private T oldValue;
        private T newValue;

        public PropertyValueChangeTimer(Action<string, T, T> propertyValueChangeAction)
        {
            this.propertyValueChangeAction = propertyValueChangeAction;
        }

        public void Enable(TimeSpan delayTime, System.Threading.SynchronizationContext synchronizationContext)
        {
            Disable();

            timer = new Timer
            {
                Interval = delayTime.TotalMilliseconds,
            };
            timer.Elapsed += OnTimerElapsed;
            this.synchronizationContext = synchronizationContext;
        }

        public void Disable()
        {
            timer?.Stop();
            if (timer != null) timer.Elapsed -= OnTimerElapsed;
            timer = null;
            synchronizationContext = null;
        }

        public void Restart(string propertyName, T oldValue, T newValue)
        {
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;

            if (timer == null)
            {
                propertyValueChangeAction?.Invoke(propertyName, oldValue, newValue);
                return;
            }

            timer.Stop();
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (synchronizationContext == null)
            {
                propertyValueChangeAction?.Invoke(propertyName, oldValue, newValue);
            }
            else
            {
                synchronizationContext.Post(state => propertyValueChangeAction?.Invoke(propertyName, oldValue, newValue), null);
            }
        }
    }
}
