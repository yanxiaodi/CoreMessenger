using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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
    }
}
