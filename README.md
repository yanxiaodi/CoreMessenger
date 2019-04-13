# CoreMessenger

A simple messenger system for WPF, UWP and Xamarin. .NET Standard supported.

This project comes from MvvmCross.Messenger and now it can be used in all the WPF, UWP and Xamarin projects without MvvmCross. The original inspiration for this code was XPlatUtils from JonathonPeppers - https://github.com/jonathanpeppers/XPlatUtils

## Usage

Install from Nuget:

```
PM> Install-Package XySoft.CoreMessenger
```

Use `MessengerHub.Instance` as the singleton pattern in your whole app domain. It provides these methods:

* Publish: 
```
public async Task Publish<TMessage>(TMessage message)
```
* Subscribe: 
```
public SubscriptionToken Subscribe<TMessage>(Action<TMessage> action, ReferenceType referenceType = ReferenceType.Weak, SubscriptionPriority priority = SubscriptionPriority.Normal, string tag = null)`
* Unsubscribe: `public async Task Unsubscribe<TMessage>(SubscriptionToken subscriptionToken)
```

### Creating the `Message` class

First, define a Message class inherited from `Message` between different components, like this:

```csharp
public class TestMessage : Message
{
    public string ExtraContent { get; private set; }
    public TestMessage(object sender, string content) : base(sender)
    {
        ExtraContent = content;
    }
}
```

Then create an instance of the `Message` in your component A, as shown below:

```csharp
var message = new TestMessage(this, "Test Content");
```

### Subscription

Define a `SubscriptionToken` instance to store the subscription. Subscribe the `Message` in your component B, like this:

```csharp
public class HomeViewModel
    {
        private readonly SubscriptionToken _subscriptionTokenForTestMessage;
        public HomeViewModel()
        {
            _subscriptionTokenForTestMessage = 
                MessengerHub.Instance.Subscribe<TestMessage>(OnTestMessageReceived,
                ReferenceType.Weak, SubscriptionPriority.Normal);
        }

        private void OnTestMessageReceived(TestMessage message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Received messages of type {message.GetType().ToString()}. Content: {message.Content}");
#endif
        }
    }
```

### Publishing the `Message`

Publish the `Message` in your component A:

```csharp
public async Task PublishMessage()
{
    await MessengerHub.Instance.Publish(new TestMessage(this, $"Hello World!"));
}
```

All done!

### Parameters

The full singnature of the `Subscribe` method is:

```csharp
public SubscriptionToken Subscribe<TMessage>(Action<TMessage> action,
            ReferenceType referenceType = ReferenceType.Weak,
            SubscriptionPriority priority = SubscriptionPriority.Normal, string tag = null) where TMessage : Message
```

You can specify these parameters:

* `ReferenceType`. The default value is `ReferenceType.Weak` so you do not need to worry about the memory leaking. Once the `SubscriptionToken` instance goes out of the scope, GC can collect it automatically(But not sure when). If you need to keep a strong reference, specify the parameter as `ReferenceType.Strong` so that GC cannot collect it.
* `SubscriptionPriority`. The default value is `SubscriptionPriority.Normal`. Sometimes it is required to control the excution orders of the subscriptions for one `Message`. In this case, specify different priorities for the subscriptions to control the excution orders. Notice that this parameter is not for different `Message`s.
* `Tag`. It is optional to inspect current status for subscriptions.

### Unsubscribe

You can use these methods to unsubscribe the subscription:

* Use `Unsubscribe` method, as shown below:
  ```csharp
  await MessengerHub.Instance.Unsubscribe<TestMessage>(_subscriptionTokenForTestMessage);
  ```
* Use `Dispose` method of the `SubscriptionToken`:
  ```csharp
  _subscriptionTokenForTestMessage.Dispose();
  ```

In many scenarios, you will not call these methods directly. If you are using the strong subscription type, it might cause memory leaking issue. So `ReferenceType.Weak` is recommended. Be aware that if the token is not stored in the context, it might be collected by GC immediately. For example:

```csharp
public void MayNotEverReceiveAMessage()
{
    var token = MessengerHub.Instance.Subscribe<TestMessage>((message) => {
        // Do something here
    });
    // token goes out of scope now
    // - so will be garbage collected *at some point*
    // - so the action may never get called
}
```

## Differences with MvvmCross.Messenger

If you are using `MvvmCross` to develop your application, please use `MvvmCross.Messenger` directly. I extracted some main methods and removed dependencies to `MvvmCross` components so it can be used in any WPF, UWP and Xamarin projects without `MvvmCross`. Also the `Publish` method is always running in background to avoid blocking the UI. Another difference is that no need to use DI to create the instance of `MessageHub` which is a singleton instance in all the app domain. It is useful if the solution contains multiple components that need communicate with each other. DI would make it more complicated.

## Thanks

* [MvvmCross](https://www.mvvmcross.com)
* [XPlatUtils](https://github.com/jonathanpeppers/XPlatUtils)
