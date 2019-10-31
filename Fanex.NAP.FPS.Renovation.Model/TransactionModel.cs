using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{
    [XmlType(TypeName = "transaction")]
    public class TransactionModel: CustomerInfoModel
    {
        [XmlAttribute]
        public int StagingTransID { get; set; }

        [XmlAttribute]
        public string DeviceCode { get; set; }

        [XmlAttribute]
        public DateTime TransDate { get; set; }      

        [XmlAttribute]
        public decimal IP { get; set; }

        [XmlAttribute]
        public int PluginID { get; set; }

        [XmlAttribute]
        public string URL { get; set; }

        [XmlAttribute]
        public string ActionResult { get; set; }

        [XmlAttribute]
        public string UserAgent { get; set; }     
        
        [XmlAttribute]
        public string FingeringCode { get; set; }

        [XmlAttribute]
        public string FingeringMoreInfo { get; set; }
     
        [XmlAttribute]
        public string UserAgentCode { get; set; }

        [XmlAttribute]
        public string BrowserName { get; set; }

        [XmlAttribute]
        public string OSName { get; set; }

        [XmlAttribute]
        public int Flagged { get; set; }        
    }
}
