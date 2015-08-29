using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLQ
{
    public class LLQNotifier
    {
        public readonly static LLQNotifier Default = new LLQNotifier();

        private readonly object _lockForSubscribersByType = new object();

        private readonly ConcurrentDictionary<Type, object> _locksForSubscription = new ConcurrentDictionary<Type, object>();

        private ConcurrentDictionary<Type, List<Subscription>> _subscriptionDictByType = new ConcurrentDictionary<Type, List<Subscription>>();

        public void Register(object subscriber)
        {
            IList<Subscription> subscriptionList = SubscriptionHandler.CreateSubscription(subscriber);

            foreach(var subscription in subscriptionList)
            {
                var subscriptionsOfType = _subscriptionDictByType.GetOrAdd(subscription.EventType, new List<Subscription>());

                lock(_locksForSubscription.GetOrAdd(subscription.EventType, new object()))
                {
                    subscriptionsOfType.Add(subscription);
                    subscriptionsOfType.Sort();
                }
            }
        }

        public void Post(object eventObj)
        {
            var subscriptionsOfType = _subscriptionDictByType[eventObj.GetType()];
            foreach(var subscription in subscriptionsOfType)
            {
                subscription.Callback();//TODO:call with param
            }
        }
    }
}
