namespace Fanex.NAP.SPU.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "Job")]
    public class JobData
    {
        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Active")]
        public bool Active { get; set; }

        [XmlAttribute(AttributeName = "FileName")]
        public string FileName { get; set; }

        [XmlElement(ElementName = "ExecutePath")]
        public List<ExecutePath> ExecutePath { get; set; }

        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

        [XmlAttribute(AttributeName = "IntervalInSecond")]
        public int IntervalInSecond { get; set; }
    }
}