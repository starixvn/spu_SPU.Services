namespace Fanex.NAP.SPU.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "Services")]
    public class JobDataRoot
    {
        [XmlElement(ElementName = "Service")]
        public List<JobDataSevices> Sevices { get; set; }
    }
}