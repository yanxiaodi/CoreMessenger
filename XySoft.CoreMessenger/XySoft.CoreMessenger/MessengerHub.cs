using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using XySoft.CoreMessenger.Dispatchers;
using XySoft.CoreMessenger.Subscriptions;

namespace XySoft.CoreMessenger
{
    /// <summary>
    /// MessageHub for publishing / subscribing messages. You should use the singelton instance of this class in the whole app domain.
    /// </summary>
    public class MessengerHub
    {
        private static readonly Lazy<MessengerHub> lazy = new Lazy<MessengerHub>(() => new MessengerHub());
        private MessengerHub() { }
        public static MessengerHub Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, BaseSubscription>> _subscriptions =
            new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, BaseSubscription>>();

        #region Subscribe
        public SubscriptionToken Subscribe<TMessage>(Action<TMessage> action, ThreadMode threadMode = ThreadMode.Current,
            ReferenceType referenceType = ReferenceType.Weak,
            SubscriptionPriority priority = SubscriptionPriority.Normal, string tag = null) where TMessage : Message
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            IDispatcher dispatcher = BuildDispatcher(threadMode);
            BaseSubscription subscription = BuildSubscription(action, dispatcher, referenceType, priority, tag);
            return SubscribeInternal(action, subscription);
        }

        private SubscriptionToken SubscribeInternal<TMessage>(Action<TMessage> action, BaseSubscription subscription)
            where TMessage : Message
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions;
            if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
            {
                messageSubscriptions = new ConcurrentDictionary<Guid, BaseSubscription>();
                _subscriptions[typeof(TMessage)] = messageSubscriptions;
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Adding subscription {subscription.Id} for {typeof(TMessage).Name}");
#endif
            messageSubscriptions[subscription.Id] = subscription;
            //PublishSubscriberChangeMessage<TMessage>(messageSubscriptions);
            return new SubscriptionToken(subscription.Id, () => UnsubscribeInternal<TMessage>(subscription.Id));
        }
        #endregion

        #region Unsubscribe
        public void Unsubscribe<TMessage>(SubscriptionToken subscriptionToken) where TMessage : Message
        {
            UnsubscribeInternal<TMessage>(subscriptionToken.Id);
        }

        private void UnsubscribeInternal<TMessage>(Guid subscriptionId) where TMessage : Message
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions;

            if (_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
            {
                if (messageSubscriptions.ContainsKey(subscriptionId))
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Removing subscription {subscriptionId}");
#endif
                    var result = messageSubscriptions.TryRemove(subscriptionId, out BaseSubscription value);
#if DEBUG
                    if (result)
                    {
                        System.Diagnostics.Debug.WriteLine($"Subscription {subscriptionId} Removed");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to remove subscription {subscriptionId}");
                    }
#endif
                }
            }

            //PublishSubscriberChangeMessage<TMessage>(messageSubscriptions);
        }
        #endregion

        #region Private methods
        private IDispatcher BuildDispatcher(ThreadMode threadMode)
        {
            switch (threadMode)
            {
                case ThreadMode.Current:
                    return new CurrentThreadDispatcher();
                case ThreadMode.Background:
                    return new BackgroundDispatcher();
                default:
                    throw new ArgumentOutOfRangeException(nameof(threadMode), "threadMode type unexpected " + threadMode);
            }
        }

        private BaseSubscription BuildSubscription<TMessage>(Action<TMessage> action, 
            IDispatcher actionRunner, ReferenceType referenceType, 
            SubscriptionPriority priority, string tag) 
            where TMessage : Message
        {
            switch (referenceType)
            {
                case ReferenceType.Strong:
                    return new StrongSubscription<TMessage>(actionRunner, action, priority, tag);
                case ReferenceType.Weak:
                    return new WeakSubscription<TMessage>(actionRunner, action, priority, tag);
                default:
                    throw new ArgumentOutOfRangeException(nameof(referenceType), "reference type unexpected " + referenceType);
            }
        }

        #endregion
    }
}
