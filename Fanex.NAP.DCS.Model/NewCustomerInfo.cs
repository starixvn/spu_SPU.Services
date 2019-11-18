using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.DCS.Model
{
    [XmlType(TypeName = "customer")]
    public class NewCustomerInfo
    {
        [XmlAttribute]
        public int AccountID { get; set; }

        [XmlAttribute]
        public int SubscriberID { get; set; }

        [XmlAttribute]
        public int IsDeposit { get; set; }

        [XmlAttribute]
        public int CustID { get; set; }

        [XmlAttribute]
        public string Username { get; set; }

        [XmlAttribute]
        public string Username2 { get; set; }

        [XmlAttribute]
        public int RoleID { get; set; }

        [XmlAttribute]
        public int CurrencyID { get; set; }

        [XmlAttribute]
        public string CurrencyName { get; set; }

        [XmlAttribute]
        public DateTime CreatedDate { get; set; }
    }
}
