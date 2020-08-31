namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class VerifySportsAgentPerformanceCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Task_VRSetting_CheckAllSportsAgentPerformance";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}