using System;
using System.Collections.Generic;
using System.Text;

namespace XySoft.CoreMessenger
{
    /// <summary>
    /// The reference type of the subscription. Weak is recommended.
    /// </summary>
    public enum ReferenceType
    {
        /// <summary>
        /// Weak Reference.
        /// </summary>
        Weak = 0,
        /// <summary>
        /// the Messenger will keep a strong reference to the callback method and Garbage Collection will not be able to remove the subscription.
        /// </summary>
        Strong = 1
    }
}
