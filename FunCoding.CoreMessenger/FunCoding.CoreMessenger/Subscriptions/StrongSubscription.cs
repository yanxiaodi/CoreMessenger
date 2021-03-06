﻿using System;
using System.Threading.Tasks;

namespace FunCoding.CoreMessenger.Subscriptions
{
    public class StrongSubscription<TMessage> : BaseSubscription where TMessage : Message
    {
        private readonly Action<TMessage> _action;

        public StrongSubscription(Action<TMessage> action,
            SubscriptionPriority priority, string tag): base(priority, tag)
        {
            _action = action;
        }
        public override async Task<bool> Invoke(object message)
        {
            var typedMessage = message as TMessage;
            if (typedMessage == null)
            {
                throw new Exception($"Unexpected message {message.ToString()}");
            }
            await Task.Run(() => _action?.Invoke(typedMessage));
            return true;
        }
    }
}
