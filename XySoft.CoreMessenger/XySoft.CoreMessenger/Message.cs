using System;
using System.Collections.Generic;
using System.Text;

namespace XySoft.CoreMessenger
{
    /// <summary>
    /// The message base class. Cannot be used directly.
    /// </summary>
    public abstract class Message
    {
        public object Sender { get; private set; }
        protected Message(object sender)
        {
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }
    }
}
