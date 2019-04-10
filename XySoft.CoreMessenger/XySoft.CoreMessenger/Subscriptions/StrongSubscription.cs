using System;
using System.Collections.Generic;
using System.Text;
using XySoft.CoreMessenger.Dispatchers;

namespace XySoft.CoreMessenger.Subscriptions
{
    public class StrongSubscription<TMessage> : BaseSubscription where TMessage : Message
    {
        public override bool IsAlive => true;
        private readonly Action<TMessage> _action;

        public StrongSubscription(IDispatcher dispatcher, Action<TMessage> action,
            SubscriptionPriority priority, string tag): base(dispatcher, priority, tag)
        {
            _action = action;
        }
        public override bool Invoke(object message)
        {
            var typedMessage = message as TMessage;
            if (typedMessage == null)
            {
                throw new Exception($"Unexpected message {message.ToString()}");
            }
            Run(() => _action?.Invoke(typedMessage));
            return true;
        }
    }
}
