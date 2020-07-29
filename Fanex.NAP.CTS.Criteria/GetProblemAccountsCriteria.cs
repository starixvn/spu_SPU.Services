using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.CTS.Criteria
{
    public class GetProblemAccountsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustIds")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "datetime")]
        public DateTime Datetime { get; set; }

        public override string GetSettingKey()
        {
            return "TicketVoiding_CTS_ProblemAccountSelect";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
