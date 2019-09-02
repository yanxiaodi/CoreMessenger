using System;
using System.Collections.Generic;
using System.Text;

namespace FunCoding.CoreMessenger
{
    public sealed class SubscriptionToken : IDisposable
    {
        public Guid Id { get; private set; }
        private readonly Action _disposeMe;
        private readonly object _dependentObject;

        public SubscriptionToken(Guid id, Action disposeMe, object dependentObject)
        {
            Id = id;
            _disposeMe = disposeMe;
            _dependentObject = dependentObject;
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
