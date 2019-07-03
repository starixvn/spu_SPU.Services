namespace Fanex.NAP.SPU.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "JobsMap")]
    public class JobDataList
    {
        [XmlElement(ElementName = "JobDataExecutingModel")]
        public List<JobData> JobDataExecutingList { get; set; }
    }
}