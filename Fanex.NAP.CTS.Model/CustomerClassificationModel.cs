namespace Fanex.NAP.CTS.Model
{
    using System;
    using System.Xml.Serialization;

    public class CustomerClassificationModel
    {
        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int CategoryId { get; set; }

        [XmlIgnore]
        public DateTime CreatedTime { get; set; }       
    }
}