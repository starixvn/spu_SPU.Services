namespace Fanex.NAP.SPU.Model
{
    using Fanex.NAP.FS.XML;
    using System;
    using System.Xml.Serialization;

    [XName("Customer")]
    public class GetCustIdModel
    {
        [XmlIgnore]
        public Int64 CTSCustID { get; set; }

        [XName("CustId")]
        public Int64 CustID { get; set; }

        [XName("CustSubID")]
        public Int64 CustSubID { get; set; }

        [XmlIgnore]
        public string UserName2 { get; set; }
    }
}
