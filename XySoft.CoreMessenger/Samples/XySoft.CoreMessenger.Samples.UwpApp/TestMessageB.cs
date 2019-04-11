using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XySoft.CoreMessenger.Samples.UwpApp
{
    public class TestMessageB : Message
    {
        public double Lat { get; private set; }
        public double Lng { get; private set; }
        public TestMessageB(object sender, double lat, double lng) : base(sender)
        {
            Lat = lat;
            Lng = lng;
        }
    }
}
