using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public class StagingRawTransactionModel
    {
        public int MinStagingTransId { get; set; }

        public int MaxStagingTransId { get; set; }

        public string APIUrl { get; set; }
    }
}
