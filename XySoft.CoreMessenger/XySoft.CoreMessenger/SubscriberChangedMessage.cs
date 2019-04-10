using System;
using System.Collections.Generic;
using System.Text;

namespace XySoft.CoreMessenger
{
    public class SubscriberChangedMessage : Message
    {
        public Type MessageType { get; private set; }
        public int SubscriberCount { get; private set; }

        public SubscriberChangedMessage(object sender, Type messageType, int countSubscribers = 0) : base(sender)
        {
            SubscriberCount = countSubscribers;
            MessageType = messageType;
        }
    }
}
