namespace Fanex.NAP.CMS.VirtualSetting.Criteria.VR_BigStakeMember
{
    using Fanex.Data.Repository;

    public class AutoMailBigCreditMemberCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Acc_Rpt_Automail_BigCreditMembers";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
