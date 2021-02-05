using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.RemoveEvidenceMultipleAccounts
{
   public class CustomerEvidenceQueueModel
    {
        [JsonIgnore]
        public long Id { get; set; }

        [JsonProperty("CTSCustID")]
        public long CTSCustId { get; set; }

        [JsonIgnore]
        public long CustId { get; set; }

        [JsonIgnore]
        public int SubscriberId { get; set; }

        [JsonProperty("EvidenceID")]
        public int EvidenceId { get; set; }

        public string Remark { get; set; }

        [JsonIgnore]
        public int ActionId { get; set; }

        public int CreatedBy { get; set; }
    }
}
