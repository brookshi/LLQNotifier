using LLQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Sample
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            rtb.TextWrapping = TextWrapping.Wrap;
            LLQNotifier.MainDispatcher = this.Dispatcher;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            LLQNotifier.Default.Register(this);
            sw.Stop();
            rtb.Text += "register: " + sw.ElapsedMilliseconds;
        }

        public void FunctionTest(object sender, RoutedEventArgs arg)
        {
            Function();
        }

        void Function()
        {
            subscriber1 subscriber1 = new subscriber1();
            subscriber2 subscriber2 = new subscriber2();
            WeakReference wr = new WeakReference(subscriber1);

            Debug.WriteLine("**********Test Async**********");
            Task.Run(() => LLQNotifier.Default.Notify(new Event1() { Flag = "async flag1" }));
            Task.Run(() => LLQNotifier.Default.Notify(new Event2() { Flag = "async flag2" }));
            Task.Run(() => LLQNotifier.Default.Notify(new Event3() { Flag = "async flag3" }));

            Debug.WriteLine("**********Test Normal**********");
            LLQNotifier.Default.Notify(new Event1() { Flag = "before gc normal flag1" });
            LLQNotifier.Default.Notify(new Event2() { Flag = "before gc normal flag2" });

            Debug.WriteLine("**********Test GC**********");
            subscriber1 = null;
            Debug.WriteLine(">>>>>>>>>>Before GC>> subscriber1 is null : " +(subscriber1 == null).ToString());
            GC.Collect();
            Debug.WriteLine(">>>>>>>>>>After GC>>wr is alive : " + wr.IsAlive.ToString());
            LLQNotifier.Default.Notify(new Event1() { Flag = "after gc normal flag1" });
            LLQNotifier.Default.Notify(new Event2() { Flag = "after gc normal flag2" });

            Debug.WriteLine("**********Test Unregister**********");
            subscriber2.Unregister();
            LLQNotifier.Default.Notify(new Event1() { Flag = "unregister flag1" });
            LLQNotifier.Default.Notify(new Event2() { Flag = "unregister flag2" });
        }

        subscriber4 subscriber4 = new subscriber4();
        subscriber5 subscriber5 = new subscriber5();
        public void PerformanceTest(object sender, RoutedEventArgs arg)
        {
            GC.Collect();
            var event4 = new Event4() { Flag = "flag4" };
            var sw = Stopwatch.StartNew();
            for (int i=0;i<100000;i++)
            {
                LLQNotifier.Default.Notify(event4);
                //subscriber4.Test4();
            }
            sw.Stop();
            rtb.Text = "\r\nNotify: " + sw.Elapsed;

            GC.Collect();
            var event5 = new Event5() { Flag = "flag5" };
            sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                LLQNotifier.Default.Notify(event5);
                //subscriber5.Test5(event5);
            }
            sw.Stop();
            rtb.Text += "\r\nNotify with param: " + sw.Elapsed;
        }

        [SubscriberCallback(typeof(Event3), NotifyPriority.Normal, ThreadMode.Main)]//cause exception if use ThreadMode.Background
        public void Test()
        {
            txt_display.Text = "test";
        }
    }
}
