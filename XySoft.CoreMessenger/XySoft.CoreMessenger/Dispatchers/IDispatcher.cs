using System;

namespace XySoft.CoreMessenger.Dispatchers
{
    public interface IDispatcher
    {
        void Invoke(Action action);
    }
}
