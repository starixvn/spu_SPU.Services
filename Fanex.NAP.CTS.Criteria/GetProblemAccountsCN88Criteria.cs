namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetProblemAccountsCN88Criteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustIds")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "datetime")]
        public DateTime? Datetime { get; set; }

        public override string GetSettingKey()
        {
            return "TicketVoiding_CTS_ProblemAccountSelect_CN88";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
