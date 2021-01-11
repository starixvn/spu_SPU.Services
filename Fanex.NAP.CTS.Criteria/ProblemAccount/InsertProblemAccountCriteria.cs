using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class InsertProblemAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ProblemAccounts")]
        public string ProblemAccountJson { get; set; }

        [SpParam(SqlParamName = "ip_IsRescan")]
        public bool IsRescan { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_InsertCategorizedProblemAccounts";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
