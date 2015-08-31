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

namespace LLQ
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubscriberCallbackAttribute : Attribute
    {
        const NotifyPriority DEFAULT_PRIORITY = NotifyPriority.Normal;

        public SubscriberCallbackAttribute(Type eventType, NotifyPriority priority = DEFAULT_PRIORITY)
        {
            EventType = eventType;
            Priority = priority;
        }

        public Type EventType { get; set; }

        public NotifyPriority Priority { get; set; }
    }
}
