using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XySoft.CoreMessenger.Dispatchers;

namespace XySoft.CoreMessenger.Subscriptions
{
    public abstract class BaseSubscription
    {
        public Guid Id { get; private set; }
        public SubscriptionPriority Priority { get; private set; }
        public string Tag { get; private set; }
        public abstract Task<bool> Invoke(object message);
        private readonly Dispatcher _dispatcher;
        protected BaseSubscription(SubscriptionPriority priority, string tag)
        {
            _dispatcher = new Dispatcher();
            Id = Guid.NewGuid();
            Priority = priority;
            Tag = tag;
        }

        protected async Task Run(Action action)
        {
            await _dispatcher.Invoke(action);
        }


    }
}
