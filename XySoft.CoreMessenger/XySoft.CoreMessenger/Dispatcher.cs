﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XySoft.CoreMessenger
{
    public class Dispatcher
    {
        public async Task Invoke(Action action)
        {
            await Task.Run(action);
        }
    }
}
