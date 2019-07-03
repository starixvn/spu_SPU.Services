namespace Fanex.NAP.SPU.Model
{
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

        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }
    }
}