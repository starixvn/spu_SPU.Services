namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.FS.XML;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [XName("r")]
    public class CustomerClassModel
    {
        [XName("id")]
        public int CustID { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

    }
}
