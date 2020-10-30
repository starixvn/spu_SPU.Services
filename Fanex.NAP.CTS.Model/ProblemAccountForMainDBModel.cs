
namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;
    using System.Xml.Serialization;

    [XName("r")]
    public class ProblemAccountForMainDBModel
    {
        [XmlIgnore]
        public int CTSCustID { get; set; }

        [XName("id")]
        public int CustID { get; set; }

        [XmlIgnore]
        public int CategoryID { get; set; }

        [XName("c")]
        public int CategoryIdMapping
        {
            get
            {
                switch (CategoryID)
                {
                    case 52:
                    case 72:
                    case 152:
                        return 8;
                    case 54:
                    case 58:
                    case 74:
                    case 78:
                    case 154:
                    case 158:
                        return 9;
                    case 53:
                    case 55:
                    case 59:
                    case 73:
                    case 75:
                    case 79:
                    case 153:
                    case 155:
                    case 159:
                        return 10;
                    case 56:
                    case 60:
                    case 76:
                    case 80:
                    case 156:
                        return 11;
                    case 57:
                    case 157:
                        return 13;
                    case -1:
                        return -1;
                    default:
                        return -99;
                }
            }
        }
    }
}
