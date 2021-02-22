using Fanex.Data.Repository;

namespace Fanex.NAP.CMS.VirtualSetting.Criteria.VR_BigStakeMember
{
    public class InsertVRMemberBigStakeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Acc_VR_MemberBigStake_Ins";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
