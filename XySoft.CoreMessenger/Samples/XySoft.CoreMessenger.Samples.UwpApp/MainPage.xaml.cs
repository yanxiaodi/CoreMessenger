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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace XySoft.CoreMessenger.Samples.UwpApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SubscriptionToken tokenA;
        private SubscriptionToken tokenAHighPriority;
        private SubscriptionToken tokenB;

        public MainPage()
        {
            this.InitializeComponent();
            this.txtMessage.Text = "test";
        }

        private void BtnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            tokenA = MessengerHub.Instance.Subscribe<TestMessageA>(OnMessageAReceived, tag: "normal");
            tokenAHighPriority = MessengerHub.Instance.Subscribe<TestMessageA>(OnMessageAHighPriorityReceived, priority: SubscriptionPriority.High, tag: "highPriority");
            Debug.WriteLine($"Subscription for Message A.");
            tokenB = MessengerHub.Instance.Subscribe<TestMessageB>(OnMessageBReceived);
            var tokenTemp = MessengerHub.Instance.Subscribe<TestMessageA>(OnMessageATempReceived);

        }

        private void OnMessageBReceived(TestMessageB obj)
        {
        }

        private void OnMessageATempReceived(TestMessageA message)
        {
            Debug.WriteLine($"Message A from temp subscription received. ExtraContent: {message.ExtraContent}");

        }

        private void OnMessageAHighPriorityReceived(TestMessageA message)
        {
            Debug.WriteLine($"Message A with high priority received. ExtraContent: {message.ExtraContent}");
        }

        private void OnMessageAReceived(TestMessageA message)
        {
            Debug.WriteLine($"Message A received. ExtraContent: {message.ExtraContent}");
        }

        private async void BtnUnsubscribe_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Before Unsubscribe, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageA>()} subscriptions for Message A");
            Debug.WriteLine($"Before Unsubscribe, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("normal")} subscriptions for Message A with normal priority");
            Debug.WriteLine($"Before Unsubscribe, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("highPriority")} subscriptions for Message A with high priority");

            await MessengerHub.Instance.Unsubscribe<TestMessageA>(tokenA);
            await MessengerHub.Instance.Unsubscribe<TestMessageA>(tokenAHighPriority);
            Debug.WriteLine($"After Unsubscribe, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageA>()} subscriptions for Message A");
            Debug.WriteLine($"After Unsubscribe, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("normal")} subscriptions for Message A with normal priority");
            Debug.WriteLine($"After Unsubscribe, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("highPriority")} subscriptions for Message A with high priority");

        }

        private async void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                await MessengerHub.Instance.Publish(new TestMessageA(this, $"Hello World! {i.ToString()}"));
            }
            
        }

        private void BtnGC_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Before GC, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageA>()} subscriptions for Message A");
            Debug.WriteLine($"Before GC, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("normal")} subscriptions for Message A with normal priority");
            Debug.WriteLine($"Before GC, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("highPriority")} subscriptions for Message A with high priority");
            Debug.WriteLine($"Before GC, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageB>()} subscriptions for Message B");

            GC.Collect();
            Debug.WriteLine($"After GC, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageA>()} subscriptions for Message A");
            Debug.WriteLine($"After GC, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("normal")} subscriptions for Message A with normal priority");
            Debug.WriteLine($"After GC, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("highPriority")} subscriptions for Message A with high priority");
            Debug.WriteLine($"After GC, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageB>()} subscriptions for Message B");

        }

        private void BtnUnscribeByToken_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Before UnscribeByToken, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageA>()} subscriptions for Message A");
            Debug.WriteLine($"Before UnscribeByToken, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("normal")} subscriptions for Message A with normal priority");
            Debug.WriteLine($"Before UnscribeByToken, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("highPriority")} subscriptions for Message A with high priority");
            Debug.WriteLine($"Before UnscribeByToken, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageB>()} subscriptions for Message B");

            tokenA.Dispose();
            tokenAHighPriority.Dispose();
            tokenB.Dispose();
            Debug.WriteLine($"After UnscribeByToken, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageA>()} subscriptions for Message A");
            Debug.WriteLine($"After UnscribeByToken, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("normal")} subscriptions for Message A with normal priority");
            Debug.WriteLine($"After UnscribeByToken, there are {MessengerHub.Instance.CountSubscriptionsForTag<TestMessageA>("highPriority")} subscriptions for Message A with high priority");
            Debug.WriteLine($"After UnscribeByToken, there are {MessengerHub.Instance.CountSubscriptionsFor<TestMessageB>()} subscriptions for Message B");

        }

        private async void BtnPerformanceTest_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                await MessengerHub.Instance.Publish(new TestMessageB(this, 100, 100));
            }
            sw.Stop();
            this.txtMessage.Text = $"Publish completed: {sw.Elapsed}";

        }
    }
}
