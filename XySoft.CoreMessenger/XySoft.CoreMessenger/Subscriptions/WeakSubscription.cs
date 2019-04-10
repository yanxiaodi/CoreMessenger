using System;
using System.Collections.Generic;
using System.Text;
using XySoft.CoreMessenger.Dispatchers;

namespace XySoft.CoreMessenger.Subscriptions
{
    public class WeakSubscription<TMessage> : BaseSubscription where TMessage : Message
    {
        private readonly WeakReference _weakReference;
        public override bool IsAlive => _weakReference.IsAlive;

        public WeakSubscription(IDispatcher dispatcher, Action<TMessage> action,
            SubscriptionPriority priority, string tag) : base(dispatcher, priority, tag)
        {
            _weakReference = new WeakReference(action);
        }

        public override bool Invoke(object message)
        {
            var typedMessage = message as TMessage;
            if (typedMessage == null)
            {
                throw new Exception($"Unexpected message {message.ToString()}");
            }
            if (!_weakReference.IsAlive)
            {
                return false;
            }
            var action = _weakReference.Target as Action<TMessage>;
            if(action == null)
            {
                return false;
            }
            Run(() => action?.Invoke(typedMessage));
            return true;
        }
    }
}
