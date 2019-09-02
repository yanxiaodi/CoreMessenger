using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunCoding.CoreMessenger.Subscriptions;

namespace FunCoding.CoreMessenger
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
        public SubscriptionToken Subscribe<TMessage>(Action<TMessage> action,
            ReferenceType referenceType = ReferenceType.Weak,
            SubscriptionPriority priority = SubscriptionPriority.Normal, string tag = null) where TMessage : Message
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            BaseSubscription subscription = BuildSubscription(action, referenceType, priority, tag);
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
            Debug.WriteLine($"Adding subscription {subscription.Id} for {typeof(TMessage).Name}");
#endif
            messageSubscriptions[subscription.Id] = subscription;
            Task.Run(async () => await PublishSubscriberChangedMessage<TMessage>(messageSubscriptions));
            return new SubscriptionToken(subscription.Id, async () => await UnsubscribeInternal<TMessage>(subscription.Id), action);
        }
        #endregion

        #region Unsubscribe
        public async Task Unsubscribe<TMessage>(SubscriptionToken subscriptionToken) where TMessage : Message
        {
            await UnsubscribeInternal<TMessage>(subscriptionToken.Id);
        }

        private async Task UnsubscribeInternal<TMessage>(Guid subscriptionId) where TMessage : Message
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions;

            if (_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
            {
                if (messageSubscriptions.ContainsKey(subscriptionId))
                {
#if DEBUG
                    Debug.WriteLine($"Removing subscription {subscriptionId}");
#endif
                    var result = messageSubscriptions.TryRemove(subscriptionId, out BaseSubscription value);
#if DEBUG
                    if (result)
                    {
                        Debug.WriteLine($"Subscription {subscriptionId} Removed");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to remove subscription {subscriptionId}");
                    }
#endif
                }
            }
            await PublishSubscriberChangedMessage<TMessage>(messageSubscriptions);
        }
        #endregion

        #region Publish
        public async Task Publish<TMessage>(TMessage message) where TMessage : Message
        {
            if (typeof(TMessage) == typeof(Message))
            {
#if DEBUG
                Debug.WriteLine("It is not recommend to use a non-specific generic class here. Please create a new class inherited from the Message class.");
#endif
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            List<BaseSubscription> toPublish = null;
            Type messageType = message.GetType();

#if DEBUG
            Debug.WriteLine($"Found {_subscriptions.Count} subscriptions of all types");
            foreach (var t in _subscriptions.Keys)
            {
                Debug.WriteLine($"Found subscriptions for {t.Name}");
            }
#endif
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions = null;
            if (_subscriptions.TryGetValue(messageType, out messageSubscriptions))
            {
#if DEBUG
                Debug.WriteLine($"Found {messageSubscriptions.Values.Count} messages of type {typeof(TMessage).Name}");
#endif
                toPublish = messageSubscriptions.Values.OrderByDescending(x => x.Priority).ToList();
            }

            if (toPublish == null || toPublish.Count == 0)
            {
#if DEBUG
                Debug.WriteLine($"Nothing registered for messages of type {messageType.Name}");
#endif
                return;
            }

            List<Guid> deadSubscriptionIds = new List<Guid>();
            foreach (var subscription in toPublish)
            {
#if DEBUG
                Debug.WriteLine($"Starting to publish messages of type {messageType.Name}");
#endif
                var result = await subscription.Invoke(message);
                if (!result)
                {
                    deadSubscriptionIds.Add(subscription.Id);
                }
            }

            if (deadSubscriptionIds.Any())
            {
                await PurgeDeadSubscriptions(messageType, deadSubscriptionIds);
            }
        }

        

        

        private async Task PublishSubscriberChangedMessage<TMessage>(ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions)
            where TMessage : Message
        {

            await PublishSubscriberChangedMessage(typeof(TMessage), messageSubscriptions);
        }

        private async Task PublishSubscriberChangedMessage(Type messageType, ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions)
        {
            var newCount = messageSubscriptions?.Count ?? 0;
            await Publish(new SubscriberChangedMessage(this, messageType, newCount));
        }

        #endregion

        #region Purge dead subscriptions

        private async Task PurgeDeadSubscriptions(Type messageType, List<Guid> deadSubscriptionIds)
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions = null;
            if (_subscriptions.TryGetValue(messageType, out messageSubscriptions))
            {
                deadSubscriptionIds.ForEach(subscriptionId =>
                {
                    if (messageSubscriptions.ContainsKey(subscriptionId))
                    {
#if DEBUG
                        Debug.WriteLine($"Subscription {subscriptionId} is dead. Removing subscription {subscriptionId}");
#endif
                        var result = messageSubscriptions.TryRemove(subscriptionId, out BaseSubscription value);
#if DEBUG
                        if (result)
                        {
                            Debug.WriteLine($"Subscription {subscriptionId} Removed");
                        }
                        else
                        {
                            Debug.WriteLine($"Failed to remove subscription {subscriptionId}");
                        }
                    
#endif
                    }
                });

            }
            await PublishSubscriberChangedMessage(messageType, messageSubscriptions);
        }
        #endregion

        #region Helper Methods
        public bool HasSubscriptionsFor<TMessage>() where TMessage : Message
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions;
            if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
            {
                return false;
            }
            return messageSubscriptions.Any();
        }

        public int CountSubscriptionsFor<TMessage>() where TMessage : Message
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions;
            if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
            {
                return 0;
            }
            return messageSubscriptions.Count;
        }

        public bool HasSubscriptionsForTag<TMessage>(string tag) where TMessage : Message
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions;
            if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
            {
                return false;
            }
            return messageSubscriptions.Any(x => x.Value.Tag == tag);
        }

        public int CountSubscriptionsForTag<TMessage>(string tag) where TMessage : Message
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions;
            if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
            {
                return 0;
            }
            return messageSubscriptions.Count(x => x.Value.Tag == tag);
        }

        public IList<string> GetSubscriptionTagsFor<TMessage>() where TMessage : Message
        {
            ConcurrentDictionary<Guid, BaseSubscription> messageSubscriptions;
            if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
            {
                return new List<string>(0);
            }
            return messageSubscriptions.Select(x => x.Value.Tag).ToList();
        }
        #endregion

        #region Private methods

        private BaseSubscription BuildSubscription<TMessage>(Action<TMessage> action, 
            ReferenceType referenceType, 
            SubscriptionPriority priority, string tag) 
            where TMessage : Message
        {
            switch (referenceType)
            {
                case ReferenceType.Strong:
                    return new StrongSubscription<TMessage>(action, priority, tag);
                case ReferenceType.Weak:
                    return new WeakSubscription<TMessage>(action, priority, tag);
                default:
                    throw new ArgumentOutOfRangeException(nameof(referenceType), "reference type unexpected " + referenceType);
            }
        }

        #endregion
    }
}
