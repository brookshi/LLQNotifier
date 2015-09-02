#region License
//   Copyright 2015 Brook Shi
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LLQ
{
    public class Subscription : IComparable
    {
        private WeakReference _subscriber;

        public object Subscriber { get { return _subscriber.Target; } }

        public bool IsSubscriberAlive { get { return _subscriber.IsAlive; } }

        public Type EventType { get; private set; }

        public NotifyPriority Priority { get; private set; }

        public ThreadMode ThreadMode { get; private set; }

        private ConditionalWeakTable<object, Action> _subscriberMethod = new ConditionalWeakTable<object, Action>();

        private MethodInfo _method;


        //>>>>>>>>>>it's the slowest
        /*var target1 = Expression.Property(Expression.Constant(_subscriber), typeof(WeakReference), "Target");
        var target = Expression.Constant(Subscriber);
        MethodCallExpression methodCall = Expression.Call(target, method);
        _callback = Expression.Lambda<Action>(methodCall).Compile();*/

        //>>>>>>>>>>it will keep the object reference
        //_callback = (Action) method.CreateDelegate(typeof(Action), Subscriber);

        //>>>>>>>>>30% slower than delegate
        //_callback = method.Invoke(Subscriber);

        public Subscription(object subscriber, MethodInfo method, Type eventType, NotifyPriority priority, ThreadMode threadMode)
        {
            _subscriber = new WeakReference(subscriber);
            Priority = priority;
            EventType = eventType;
            ThreadMode = threadMode;
            InitMethod(subscriber, method);
        }

        void InitMethod(object subscriber, MethodInfo method)
        {
            var paramsInfo = method.GetParameters();

            if (paramsInfo.Length == 0)
            {
                _subscriberMethod.Add(subscriber, (Action)method.CreateDelegate(typeof(Action), subscriber));
            }
            else
            {
                _method = method;
            }
        }

        public void ExecCallback(object param)
        {
            if (!IsSubscriberAlive || Subscriber == null)
                return;

            Action callback;
            if (_subscriberMethod.TryGetValue(Subscriber, out callback))
            {
                callback();
            }
            else if(_method != null)
            {
                _method.Invoke(Subscriber, new []{ param });
            }
        }

        public int CompareTo(object obj)
        {
            var subscription = obj as Subscription;
            if (subscription == null)
                throw new ArgumentException();

            return (int)Priority > (int)subscription.Priority ? -1 : 1;
        }

        public override bool Equals(object obj)
        {
            var subscription = obj as Subscription;
            return subscription != null && subscription.Subscriber != null && subscription.Subscriber.Equals(Subscriber);
        }

        public override int GetHashCode()
        {
            return Subscriber == null ? -1 : Subscriber.GetHashCode();
        }
    }
}
