using System;
using System.Collections.Generic;
using System.Text;
using XySoft.CoreMessenger.Dispatchers;

namespace XySoft.CoreMessenger.Subscriptions
{
    public abstract class BaseSubscription
    {
        public Guid Id { get; private set; }
        public SubscriptionPriority Priority { get; private set; }
        public string Tag { get; private set; }
        public abstract bool IsAlive { get; }
        public abstract bool Invoke(object message);
        private readonly IDispatcher _dispatcher;
        protected BaseSubscription(IDispatcher dispatcher, SubscriptionPriority priority, string tag)
        {
            _dispatcher = dispatcher;
            Id = Guid.NewGuid();
            Priority = priority;
            Tag = tag;
        }

        protected void Run(Action action)
        {
            _dispatcher.Invoke(action);
        }


    }
}
