using System;
using System.Collections.Generic;
using System.Text;

namespace XySoft.CoreMessenger
{
    /// <summary>
    /// Invoke thread mode.
    /// </summary>
    public enum ThreadMode
    {
        /// <summary>
        /// Messages will be passed directly on the current Publish thread. 
        /// </summary>
        Current = 0,
        /// <summary>
        /// Messages will be queued for thread pool processing.
        /// </summary>
        Background = 1
    }
}
