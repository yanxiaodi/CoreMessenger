using System;
using System.Threading.Tasks;

namespace FunCoding.CoreMessenger.Subscriptions
{
    public abstract class BaseSubscription
    {
        public Guid Id { get; private set; }
        public SubscriptionPriority Priority { get; private set; }
        public string Tag { get; private set; }
        public abstract Task<bool> Invoke(object message);
        protected BaseSubscription(SubscriptionPriority priority, string tag)
        {
            Id = Guid.NewGuid();
            Priority = priority;
            Tag = tag;
        }
    }
}
