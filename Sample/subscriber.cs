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

using LLQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    public class subscriber1
    {
        public subscriber1()
        {
            LLQNotifier.Default.Register(this);
        }

        [SubscriberCallback(typeof(Event1))]
        public void Test1()
        {
            Debug.WriteLine("->>>>>>>>>>subscriber1>>Test1");
        }

        [SubscriberCallback(typeof(Event1), NotifyPriority.Highest)]
        public void Test2(Event1 e)
        {
            Debug.WriteLine("->>>>>>>>>>subscriber1>>Test2 @@@@ " + e.Flag);
        }
    }

    public class subscriber2
    {
        public subscriber2()
        {
            LLQNotifier.Default.Register(this);
        }

        [SubscriberCallback(typeof(Event2), NotifyPriority.Lowest)]
        public void Test3()
        {
            Debug.WriteLine("->>>>>>>>>>subscriber2>>Test3");
        }

        [SubscriberCallback(typeof(Event1), NotifyPriority.AboveNormal)]
        public void Test4()
        {
            Debug.WriteLine("->>>>>>>>>>subscriber2>>Test4");
        }
    }
}
