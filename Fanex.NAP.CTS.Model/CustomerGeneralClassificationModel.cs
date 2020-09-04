namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;
    using System.Xml.Serialization;

    [XName("r")]
    public class CustomerGeneralClassificationModel
    {
        [XName("id")]
        public int CustId { get; set; }


        [XmlIgnore]
        public int CategoryId { get; set; }

        [XName("c")]
        public int CategoryIdMapping
        {
            get
            {
                switch (CategoryId)
                {
                    case 205:
                        return 1;
                    case 202:
                        return 2;
                    case 206:
                        return 5;
                    case 204:
                        return 6;
                    case 203:
                        return 7;
                    default:
                        return -1;
                }
            }
        }
    }
}
