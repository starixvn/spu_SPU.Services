using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Model
{
    public class TransactionModel
    {
        [JsonIgnore]
        public sbyte BatchId { get; set; }

        public long TransId { get; set; }
    }
}
