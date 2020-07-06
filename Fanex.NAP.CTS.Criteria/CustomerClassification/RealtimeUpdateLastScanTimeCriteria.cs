using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class RealtimeUpdateLastScanTimeCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CustomerClassification_UpdateLastScanTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
