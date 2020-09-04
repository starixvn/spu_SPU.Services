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
                    case 28:
                    case 102:
                        return 8;
                    case 30:
                    case 104:
                    case 37:
                    case 108:
                        return 9;
                    case 29:
                    case 103:
                    case 31:
                    case 105:
                    case 38:
                    case 109:
                        return 10;
                    case 32:
                    case 106:
                        return 11;
                    case 36:
                    case 107:
                        return 13;
                    default:
                        return -1;
                }
            }
        }
    }
}
