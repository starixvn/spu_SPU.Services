
namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;
    using System;
    using System.Xml.Serialization;

    [XName("r")]
    public class ProblemCustomerForMainDBModel
    { 

        [XName("id")]
        public int CustID { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

        [XmlIgnore]
        public DateTime ScannedTime { get; set; }
    }
}
