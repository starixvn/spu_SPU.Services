using Fanex.Data.Repository;

namespace Fanex.NAP.CMS.VirtualSetting.Criteria.VR_BigStakeMember
{
    public class UpdateWhitelistVRMemberBigStakeCriteria : CriteriaBase
    {
        public string CustIdList { get; set; }

        public int Type { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_MemberBigStake_WhiteList_Upd";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
