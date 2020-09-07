
namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateUnflagAbnormalAgentCriteria : CriteriaBase
    {
        public string CustIDList { get; set; }

        public int Type { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_AbnormalAgent_Unflag";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDList);
        }
    }
}
