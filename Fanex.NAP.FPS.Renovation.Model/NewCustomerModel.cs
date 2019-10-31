using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{
    [XmlType(TypeName = "customer")]
    public class NewCustomerModel
    {
        [XmlAttribute]
        public long AccountID { get; set; }

        [XmlAttribute]
        public int SubscriberID { get; set; }

        [XmlAttribute]
        public string SubscriberName { get; set; }

        [XmlAttribute]
        public string LoginName { get; set; }

        [XmlAttribute]
        public string LoginNameWithPrefix { get; set; }
    }
}
