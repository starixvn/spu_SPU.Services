namespace Fanex.NAP.CMS.VirtualSetting.Criteria.VR_Agent
{ 
    using Fanex.Data.Repository;

    public class InsertNewVRAgentCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Acc_VR_AgentAutoAdjustment_Ins";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
