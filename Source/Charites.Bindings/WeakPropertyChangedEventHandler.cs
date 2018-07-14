// Copyright (C) 2018 Fievus
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Charites.Windows.Mvc.Bindings
{
    internal sealed class WeakPropertyChangedEventHandler : IEquatable<PropertyChangedEventHandler>
    {
        private readonly WeakReference targetReference;
        private readonly MethodInfo method;
        private readonly Action<object, object, PropertyChangedEventArgs> action;

        public bool IsAlive => targetReference == null || targetReference.IsAlive;

        public WeakPropertyChangedEventHandler(PropertyChangedEventHandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            targetReference = handler.Target == null ? null : new WeakReference(handler.Target);
            method = handler.Method;
            action = CreateAction(handler);
        }

        public void Invoke(object sender, PropertyChangedEventArgs e)
        {
            var target = targetReference?.Target;
            if (targetReference != null && target == null) return;

            action(target, sender, e);
        }


        public bool Equals(PropertyChangedEventHandler other)
        {
            if (other == null) return false;
            if (other.Target == null) return targetReference == null && other.Method == method;

            return other.Target == targetReference?.Target && other.Method == method;
        }

        private Action<object, object, PropertyChangedEventArgs> CreateAction(PropertyChangedEventHandler handler)
        {
            var target = Expression.Parameter(typeof(object), "target");
            var sender = Expression.Parameter(typeof(object), "sender");
            var e = Expression.Parameter(typeof(PropertyChangedEventArgs), "e");

            var instance = handler.Target == null ? null : Expression.Convert(target, handler.Target.GetType());

            return Expression.Lambda<Action<object, object, PropertyChangedEventArgs>>(Expression.Call(instance, handler.Method, sender, e), target, sender, e).Compile();
        }
    }
}
