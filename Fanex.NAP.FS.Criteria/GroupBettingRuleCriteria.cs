using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Criteria
{
    public class GroupBettingRuleCriteria : NonQueryCommand
    {
        public string MatchIds { get; set; }

        public string GroupBettingAccountsXML { get; set; }

        public override string GetSettingKey() => "FS_FraudCase_GroupBetting_Rule";

        public override bool IsValid() => true;
    }
}
