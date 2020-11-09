namespace Fanex.NAP.CTS.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetCustomerEvidenceRawModel
    {
        public long ID { get; set; }

        public int CTSCustID { get; set; }

        public int EvidenceID { get; set; }

        public string Remark { get; set; }

        public bool IsOverwrite { get; set; }

        public string FileKey { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
