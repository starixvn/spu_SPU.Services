namespace Fanex.NAP.CTS.Model
{
    using System;
    using System.Xml.Serialization;

    public class CustomerClassificationModel
    {
        [XmlIgnore]
        public int BatchID { get; set; }

        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        [XmlIgnore]
        public int CategoryId { get; set; }

        [XmlIgnore]
        public DateTime CreatedTime { get; set; }       
    }
}