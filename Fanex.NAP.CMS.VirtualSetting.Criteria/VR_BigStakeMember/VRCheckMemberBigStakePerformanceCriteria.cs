using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CMS.VirtualSetting.Criteria.VR_BigStakeMember
{
    public class VRCheckMemberBigStakePerformanceCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Task_VRSetting_CheckMemberBigStakePerformance";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
