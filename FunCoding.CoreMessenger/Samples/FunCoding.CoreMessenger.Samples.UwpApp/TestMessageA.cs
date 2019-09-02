using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunCoding.CoreMessenger.Samples.UwpApp
{
    public class TestMessageA : Message
    {
        public string ExtraContent { get; private set; }
        public TestMessageA(object sender, string content) : base(sender)
        {
            ExtraContent = content;
        }
    }
}
