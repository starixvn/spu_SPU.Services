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
        public int MinTransId { get; set; }

        public int MaxTransId { get; set; }

        public string APIUrl { get; set; }
    }
}
