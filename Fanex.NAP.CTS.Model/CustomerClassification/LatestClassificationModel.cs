namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;
    using System;
    using System.Xml.Serialization;

    [XName("r")]
    public class LatestClassificationModel
    { 

        [XName("id")]
        public long CustId { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

        [XmlIgnore]
        public DateTime ScannedTime { get; set; }

        /// <summary>
        /// TODO: Remove these 
        /// </summary>
        [XmlIgnore]
        public bool IsRescanPA { get; set; }

        [XmlIgnore]
        public bool IsRescanNormal { get; set; }
    }
}
