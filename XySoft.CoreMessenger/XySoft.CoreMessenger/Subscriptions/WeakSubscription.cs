using System;
using System.Collections.Generic;
using System.Text;
using XySoft.CoreMessenger.Dispatchers;

namespace XySoft.CoreMessenger.Subscriptions
{
    public class WeakSubscription<TMessage> : BaseSubscription where TMessage : Message
    {
        private readonly WeakReference<Action<TMessage>> _weakReference;

        public WeakSubscription(IDispatcher dispatcher, Action<TMessage> action,
            SubscriptionPriority priority, string tag) : base(dispatcher, priority, tag)
        {
            _weakReference = new WeakReference<Action<TMessage>>(action);
            action = null;
        }

        public override bool Invoke(object message)
        {
            var typedMessage = message as TMessage;
            if (typedMessage == null)
            {
                throw new Exception($"Unexpected message {message.ToString()}");
            }
            if (!_weakReference.TryGetTarget(out Action<TMessage> action))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Subscription {Id} for {typeof(TMessage)} has been reclaimed by garbage collection");
#endif
                return false;
            }
            Run(() => action?.Invoke(typedMessage));
            return true;
        }
    }
}
