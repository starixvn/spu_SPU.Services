namespace Fanex.NAP.SPU.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "Service")]
    public class JobDataSevices
    {
        [XmlElement(ElementName = "Job")]
        public List<JobData> Jobs { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Active")]
        public bool Active { get; set; }
    }
}