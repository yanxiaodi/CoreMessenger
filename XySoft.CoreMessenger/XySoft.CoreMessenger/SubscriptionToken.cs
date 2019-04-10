using System;
using System.Collections.Generic;
using System.Text;

namespace XySoft.CoreMessenger
{
    public sealed class SubscriptionToken : IDisposable
    {
        public Guid Id { get; private set; }
        private readonly Action _disposeMe;

        public SubscriptionToken(Guid id, Action disposeMe)
        {
            Id = id;
            _disposeMe = disposeMe;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _disposeMe();
            }
        }
    }
}
