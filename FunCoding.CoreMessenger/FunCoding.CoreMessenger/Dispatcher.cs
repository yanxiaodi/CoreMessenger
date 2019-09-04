using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunCoding.CoreMessenger
{
    public class Dispatcher
    {
        public Task Invoke(Action action)
        {
            return Task.Run(action);
        }
    }
}
