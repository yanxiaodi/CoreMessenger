using System;

namespace XySoft.CoreMessenger.Dispatchers
{
    public class CurrentThreadDispatcher : IDispatcher
    {
        public void Invoke(Action action)
        {
            action?.Invoke();
        }
    }
}
