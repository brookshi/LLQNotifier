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

using System.Reflection;
using System.Collections.Generic;

namespace LLQ
{
    public class SubscriptionHandler
    {
        public static IList<Subscription> CreateSubscription(object subscriber)
        {
            IList<Subscription> subscriptionList = new List<Subscription>();

            var methodInfos = subscriber.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var methodInfo in methodInfos)
            {
                var paramsInfo = methodInfo.GetParameters();
                var returnType = methodInfo.ReturnType;

                if (!methodInfo.IsDefined(typeof(SubscriberCallbackAttribute)) || paramsInfo.Length > 1 || returnType != typeof(void))
                    continue;

                var attr = methodInfo.GetCustomAttribute<SubscriberCallbackAttribute>(true);

                subscriptionList.Add(new Subscription(subscriber, methodInfo, attr.EventType, attr.Priority, attr.ThreadMode));
            }

            return subscriptionList;
        }
    }
}
