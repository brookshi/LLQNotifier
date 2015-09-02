#LLQNotifier

LLQNotifier is notify/subscribe event manager for UWP (Universal Windows Platform), like [EventBus](https://github.com/greenrobot/EventBus)

Features
--------
- Easy use, 3 step
- Low coupling, notifier and subscriber are hiden for each other
- Quick, <1 ms to call method
- Weak Reference, subscriber are weak reference, so it will be removed after GC
- Priority control, for the same event, you can set priority for subscribers.
- Thread Mode, exec callback in main/current/background thread.

RoadMap
--------
- Supports sticky event.


Usage
--------
####Step 1, declare event 
``` java
public class Event1
{
    public string Flag { get; set; }
}
```
        		
####Step 2, register subscriber
``` java
public class subscriber
{
    public subscriber()
    {
        LLQNotifier.Default.Register(this);
    }

    public void Unregister()
    {
        LLQNotifier.Default.Unregister(this);
    }

    [SubscriberCallback(typeof(Event1), NotifyPriority.Lowest, ThreadMode.Background)]//if thread mode is Main, should set LLQNotifier.MainDispatcher = [UI Dispatcher]
    public void Test()
    {
        Debug.WriteLine("->>>>>>>>>>subscriber>>Test");
    }
}
```
				
####Step 3, Notify
``` java
LLQNotifier.Default.Notify(new Event1() { Flag = "flag" });
```

NuGet
--------
``` java
PM> Install-Package LLQNotifier 
```


License
--------
``` 
Copyright 2015 Brook Shi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. 
```
