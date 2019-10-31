namespace Fanex.NAP.FPS.Renovation.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class InsertIPTableLogCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UpdatedDate")]
        public DateTime UpdatedDate { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_UpdateIPTableLog";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
