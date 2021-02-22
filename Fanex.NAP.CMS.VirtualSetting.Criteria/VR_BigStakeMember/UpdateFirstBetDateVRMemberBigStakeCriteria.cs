using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CMS.VirtualSetting.Criteria.VR_BigStakeMember
{
    public class UpdateFirstBetDateVRMemberBigStakeCriteria : CriteriaBase
    {
        public string CustIdList { get; set; }

        public int Type { get; set; }

        public DateTime BetDate { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_MemberBigStake_FirstBetDate_Upd";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
