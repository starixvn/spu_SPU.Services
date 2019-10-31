using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public class OSModel
    {
        public int OSId { get; set; }

        public string OSName { get; set; }

        public int IsExisting { get; set; }

        public string UserAgentCode { get; set; }
    }
}
