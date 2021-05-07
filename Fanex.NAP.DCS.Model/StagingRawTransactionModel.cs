using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.DCS.Model
{
    public class StagingRawTransactionModel
    {
        public long MinTransId { get; set; }

        public long MaxTransId { get; set; }

        public string APIUrl { get; set; }
    }
}
