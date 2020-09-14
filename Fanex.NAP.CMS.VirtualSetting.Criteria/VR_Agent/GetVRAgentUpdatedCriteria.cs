namespace Fanex.NAP.CMS.VirtualSetting.Criteria.VR_Agent
{    
    using Fanex.Data.Repository;   

    public class GetVRAgentUpdatedCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "Acc_Rpt_Automail_AgentsGotVRUpdated";

        public override bool IsValid()
            => true;
    }
}