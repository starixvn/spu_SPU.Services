using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Model
{
   public class TransactionMonitoringModel
    {
        public string SubscriberName { get; set; }

        public int SubscriberId { get; set; }

        public DateTime TransDate { get; set; }

        public int Count { get; set; }
    }
}
