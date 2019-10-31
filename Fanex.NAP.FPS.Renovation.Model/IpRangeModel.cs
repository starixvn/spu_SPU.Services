using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{
    [XmlType(TypeName = "ip")]
    public class IpRangeModel
    {
        [XmlAttribute]
        public Int64 IPRangeLocationCode { get; set; }       

        [XmlAttribute]
        public string FromIP { get; set; }

        [XmlAttribute]
        public string ToIP { get; set; }

        [XmlAttribute]
        public string CountryCode { get; set; }

        [XmlAttribute]
        public string CountryName { get; set; }

        [XmlAttribute]
        public string Region { get; set; }

        [XmlAttribute]
        public string City { get; set; }

        [XmlAttribute]
        public string ISPName { get; set; }

        [XmlAttribute]
        public int Id { get; set; }
    }
}
