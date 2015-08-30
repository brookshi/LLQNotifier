using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LLQ
{
    public class LLQNotifier
    {
        public readonly static LLQNotifier Default = new LLQNotifier();

        private readonly object _lockForSubscribersByType = new object();

        private readonly ConcurrentDictionary<Type, object> _locksForSubscription = new ConcurrentDictionary<Type, object>();

        private readonly object _lockForSubscribersList = new object();

        private ConcurrentDictionary<Type, List<Subscription>> _subscriptionDictByType = new ConcurrentDictionary<Type, List<Subscription>>();

        private ConditionalWeakTable<object, List<Type>> _subscriberDictWithType = new ConditionalWeakTable<object, List<Type>>();

        public void Register(object subscriber)
        {
            List<Type> subscriptTypes = new List<Type>();
            if (_subscriberDictWithType.ContainsKey(subscriber))
            {
                throw new ArgumentException("subscriber had been registed");
            }

            IList<Subscription> subscriptionList = SubscriptionHandler.CreateSubscription(subscriber);

            foreach(var subscription in subscriptionList)
            {
                var subscriptionsOfType = _subscriptionDictByType.GetOrAdd(subscription.EventType, new List<Subscription>());

                lock(_locksForSubscription.GetOrAdd(subscription.EventType, new object()))
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
            var subscriptionsOfType = _subscriptionDictByType[eventObj.GetType()];
            foreach(var subscription in subscriptionsOfType)
            {
                if (subscription.IsSubscriberAlive && _subscriberDictWithType.ContainsKey(subscription.Subscriber))
                {
                    subscription.ExecCallback(eventObj);
                }
            }
        }
    }
}
