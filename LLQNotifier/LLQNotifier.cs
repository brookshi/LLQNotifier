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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace LLQ
{
    public class LLQNotifier
    {
        public readonly static LLQNotifier Default = new LLQNotifier();

        public static CoreDispatcher MainDispatcher { get; set; }

        static SynchronizationContext _syncContext;

        private readonly object _lockForSubscribersByType = new object();

        private readonly ConcurrentDictionary<Type, object> _locksForSubscription = new ConcurrentDictionary<Type, object>();

        private readonly object _lockForSubscribersList = new object();

        private ConcurrentDictionary<Type, List<Subscription>> _subscriptionDictByType = new ConcurrentDictionary<Type, List<Subscription>>();

        private ConditionalWeakTable<object, List<Type>> _subscriberDictWithType = new ConditionalWeakTable<object, List<Type>>();

        public void Register(object subscriber)
        {
            if (_syncContext == null && SynchronizationContext.Current != null)
                _syncContext = SynchronizationContext.Current;

            List<Type> subscriptTypes = new List<Type>();
            if (_subscriberDictWithType.ContainsKey(subscriber))
            {
                throw new ArgumentException("subscriber had been registed");
            }

            IList<Subscription> subscriptionList = SubscriptionHandler.CreateSubscription(subscriber);

            foreach (var subscription in subscriptionList)
            {
                var subscriptionsOfType = _subscriptionDictByType.GetOrAdd(subscription.EventType, new List<Subscription>());

                lock (_locksForSubscription.GetOrAdd(subscription.EventType, new object()))
                {
                    subscriptionsOfType.Add(subscription);
                    subscriptionsOfType.Sort();
                }

                if (!subscriptTypes.Contains(subscription.EventType))
                {
                    subscriptTypes.Add(subscription.EventType);
                }
            }

            _subscriberDictWithType.Add(subscriber, subscriptTypes);
        }



        public void Unregister(object subscriber)
        {
            List<Type> types = null;
            if(_subscriberDictWithType.TryGetValue(subscriber, out types))
            {
                foreach(var type in types)
                {
                    RemoveSubscription(type, subscriber);
                }
            }
        }

        void RemoveSubscription(Type eventType, object subscriber)
        {
            List<Subscription> subscriptions;
            if (_subscriptionDictByType.TryGetValue(eventType, out subscriptions))
            {
                lock (_locksForSubscription.GetOrAdd(eventType, new object()))
                {
                    subscriptions.RemoveAll(o => o.Subscriber == subscriber);
                }
            }
        }

        public void Notify(object eventObj)
        {
            if (!_subscriptionDictByType.ContainsKey(eventObj.GetType()))
                return;

            var subscriptionsOfType = _subscriptionDictByType[eventObj.GetType()];
            foreach(var subscription in subscriptionsOfType)
            {
                if (subscription.IsSubscriberAlive && _subscriberDictWithType.ContainsKey(subscription.Subscriber))
                {
                    DispatchNotification(subscription, eventObj);
                }
            }
        }

        void DispatchNotification(Subscription subscription, object eventObj)
        {
            switch(subscription.ThreadMode)
            {
                case ThreadMode.Current:
                    subscription.ExecCallback(eventObj);
                    return;
                case ThreadMode.Background:
                    Task.Run(() => subscription.ExecCallback(eventObj));
                    return;
                case ThreadMode.Main:
                    if (SynchronizationContext.Current == null)
                    {
                        if (MainDispatcher != null)
                            MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => subscription.ExecCallback(eventObj)).AsTask();
                        else if (_syncContext != null)
                            _syncContext.Post((a) => subscription.ExecCallback(eventObj), null);
                        else
                            throw new Exception("cannot get ui dispatcher");
                    }
                    else
                    {
                        subscription.ExecCallback(eventObj);
                    }
                    return;
                default:
                    throw new Exception(subscription.ThreadMode.ToString() + " is not supported");
            }
        }
    }
}
