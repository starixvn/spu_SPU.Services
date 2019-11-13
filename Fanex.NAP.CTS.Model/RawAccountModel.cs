using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model
{
    public class RawAccountModel
    {
        public int BatchId { get; set; }

        public int AccountId { get; set; }

        public string LoginNameWithPrefix { get; set; }

        public int SubscriberId { get; set; }
    }
}
