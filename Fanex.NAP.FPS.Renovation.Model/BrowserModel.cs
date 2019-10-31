using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public class BrowserModel
    {
        public int BrowserId { get; set; }

        public string BrowserName { get; set; }

        public int IsExisting { get; set; }

        public string UserAgentCode { get; set; }
    }
}
