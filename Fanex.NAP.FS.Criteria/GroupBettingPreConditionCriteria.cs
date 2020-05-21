using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Criteria
{
    public class GroupBettingPreConditionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "FS_FraudCase_GroupBetting_PreCondition";

        public override bool IsValid() => true;
    }
}
