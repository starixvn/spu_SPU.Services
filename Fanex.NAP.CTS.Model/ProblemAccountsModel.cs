namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;
    using System.Xml.Serialization;

    [XName("r")]
    public class ProblemAccountsModel
    {
        [XName("id")]
        public int CustID { get; set; }

        [XmlIgnore]
        public int ReasonID { get; set; }

        [XName("c")]
        public int CategoryIdMapping
        {
            get
            {
                switch (ReasonID)
                {
                    case 52:
                    case 152:
                        return 8;
                    case 54:
                    case 154:
                        return 9;
                    case 53:
                    case 153:
                    case 55:
                    case 155:
                    case 59:
                    case 159:
                        return 10;
                    case 56:
                    case 156:
                        return 11;
                    case 57:
                    case 157:
                        return 13;
                    default:
                        return ReasonID;
                }
            }
        }
    }
}
