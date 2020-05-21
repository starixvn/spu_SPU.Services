using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Criteria
{
    public class HedgingRuleCriteria : NonQueryCommand
    {
        public string MatchIds { get; set; }

        public string HedgingAccountsXML { get; set; }

        public override string GetSettingKey() => "FS_FraudCase_Hedging_Rule";

        public override bool IsValid() => true;
    }
}
