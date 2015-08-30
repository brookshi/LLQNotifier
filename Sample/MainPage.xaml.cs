using LLQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("**********Test Normal**********");
            subscriber1 subscriber1 = new subscriber1();
            subscriber2 subscriber2 = new subscriber2();
            WeakReference wr = new WeakReference(subscriber1);
            LLQNotifier.Default.Notify(new Event1() { Flag = "flag1" });
            LLQNotifier.Default.Notify(new Event2() { Flag = "flag2" });

            Debug.WriteLine("**********Test GC**********");
            subscriber1 = null;
            Debug.WriteLine(">>>>>>>>>>Before GC>>" + (subscriber1 == null).ToString());
            GC.Collect();
            Debug.WriteLine(">>>>>>>>>>After GC>>" + wr.IsAlive.ToString());
            LLQNotifier.Default.Notify(new Event1() { Flag = "flag1" });
            LLQNotifier.Default.Notify(new Event2() { Flag = "flag2" });

            Debug.WriteLine("**********Test Unregister**********");
            subscriber2.Unregister();
            LLQNotifier.Default.Notify(new Event1() { Flag = "flag1" });
            LLQNotifier.Default.Notify(new Event2() { Flag = "flag2" });
        }
    }
}
