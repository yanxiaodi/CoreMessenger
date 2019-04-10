using System;
using System.Threading.Tasks;

namespace XySoft.CoreMessenger.Dispatchers
{
    public class BackgroundDispatcher : IDispatcher
    {
        public void Invoke(Action action)
        {
            Task.Run(action);
        }
    }
}
