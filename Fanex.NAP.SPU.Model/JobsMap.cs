namespace Fanex.NAP.SPU.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "JobsMap")]
    public class JobsMap
    {
        [XmlElement(ElementName = "JobDataExecutingModel")]
        public List<JobDataExecutingModel> JobDataExecutingModels { get; set; }
    }
}