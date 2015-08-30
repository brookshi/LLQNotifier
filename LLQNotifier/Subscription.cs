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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLQ
{
    public class Subscription : IComparable
    {
        private WeakReference _subscriber;
        public object Subscriber { get { return _subscriber.Target; } }

        public bool IsSubscriberAlive { get { return _subscriber.IsAlive; } }

        private Type _eventType;
        public Type EventType { get { return _eventType; } }

        private int _priority;
        public int Priority { get { return _priority; } }

        private Action _callback;
        public Action<object> _callbackWithParam;

        public void ExecCallback(object param)
        {
            if (!IsSubscriberAlive)
                return;

            if(_callback != null)
            {
                _callback();
            }
            else if(_callbackWithParam != null)
            {
                _callbackWithParam(param);
            }
        }

        public Subscription(object subscriber, Action callback, Type eventType, int priority)
        {
            _subscriber = new WeakReference(subscriber);
            _callback = callback;
            _priority = priority;
            _eventType = eventType;
        }

        public Subscription(object subscriber, Action<object> callbackWithParam, Type eventType, int priority)
        {
            _subscriber = new WeakReference(subscriber);
            _callbackWithParam = callbackWithParam;
            _priority = priority;
            _eventType = eventType;
        }

        public int CompareTo(object obj)
        {
            var subscription = obj as Subscription;
            if (subscription == null)
                throw new ArgumentException();

            return Priority > subscription.Priority ? -1 : 1;
        }
    }
}
