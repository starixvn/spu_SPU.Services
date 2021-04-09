namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;
    using System;
    using System.Xml.Serialization;

    [XName("r")]
    public class DangerLevelModel
    { 
        [XName("id")]
        public long CustId { get; set; }

        [XName("l")]
        public int DangerLevel { get; set; }

        [XmlIgnore]
        public DateTime ScannedTime { get; set; }
    }
}
