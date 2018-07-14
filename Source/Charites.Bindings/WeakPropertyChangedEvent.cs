// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Charites.Windows.Mvc.Bindings
{
    internal sealed class WeakPropertyChangedEvent
    {
        private readonly List<WeakPropertyChangedEventHandler> handlers = new List<WeakPropertyChangedEventHandler>();

        public void AddHandler(PropertyChangedEventHandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (handlers)
            {
                RemoveGarbageCollectedHandlers();
                handlers.Add(new WeakPropertyChangedEventHandler(handler));
            }
        }

        public void RemoveHandler(PropertyChangedEventHandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (handlers)
            {
                RemoveGarbageCollectedHandlers();
                handlers.RemoveAll(h => h.Equals(handler));
            }
        }

        public void Raise(object sender, PropertyChangedEventArgs e)
        {
            WeakPropertyChangedEventHandler[] raisedHandlers;
            lock (handlers)
            {
                RemoveGarbageCollectedHandlers();
                raisedHandlers = handlers.ToArray();
            }

            foreach (var raisedHandler in raisedHandlers)
            {
                raisedHandler.Invoke(sender, e);
            }
        }

        private void RemoveGarbageCollectedHandlers() => handlers.RemoveAll(h => !h.IsAlive);
    }
}
