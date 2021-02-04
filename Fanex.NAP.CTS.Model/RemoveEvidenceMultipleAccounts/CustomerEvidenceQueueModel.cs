using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.RemoveEvidenceMultipleAccounts
{
   public class CustomerEvidenceQueueModel
    {
        public long Id { get; set; }

        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public int SubscriberId { get; set; }

        public int EvidenceId { get; set; }

        public string Remark { get; set; }

        public int ActionId { get; set; }
    }
}
