namespace Fanex.NAP.FS.Model
{
    using Fanex.NAP.FS.XML;
    using System.Xml.Serialization;

    [XName("CustomerEvidence")]
    public class FlaggedCustomer
    {
        public int CustId { get; set; }

        public string EvidenceCode { get; set; }

        [XmlIgnore]
        public string EvidenceName { get; set; }
    }
}