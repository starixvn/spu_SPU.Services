namespace Fanex.NAP.CMS.VirtualSetting.Criteria.VR_Agent
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetVRAgentUpdatedCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "processDate")]
        public DateTime ProcessDate { get; set; }

        public override string GetSettingKey()
            => "Acc_Rpt_Automail_AgentsGotVRUpdated";

        public override bool IsValid()
            => true;
    }
}