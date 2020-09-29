
namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateFlagAbnormalAgentCriteria : CriteriaBase
    {
        public int Type { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_AbnormalAgent_Flag";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
