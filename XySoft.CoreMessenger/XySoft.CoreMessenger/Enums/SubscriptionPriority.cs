using System;
using System.Collections.Generic;
using System.Text;

namespace XySoft.CoreMessenger
{
    /// <summary>
    /// The priority of subscription. For the same message, all the subscriptions will be invoked according to the priority orders.
    /// </summary>
    public enum SubscriptionPriority
    {
        /// <summary>
        /// Lowest priority.
        /// </summary>
        Lowest = 0,
        /// <summary>
        /// Between Low and Lowest priority.
        /// </summary>
        VeryLow = 1,
        /// <summary>
        /// Low priority.
        /// </summary>
        Low = 2,
        /// <summary>
        /// Normal priority.
        /// </summary>
        Normal = 3,
        /// <summary>
        /// Between High and Normal priority.
        /// </summary>
        AboveNormal = 4,
        /// <summary>
        /// High priority.
        /// </summary>
        High = 5,
        /// <summary>
        /// Between Highest and High priority.
        /// </summary>
        VeryHigh = 6,
        /// <summary>
        /// Highest priority.
        /// </summary>
        Highest = 7
    }
}
