using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class InsertProblemAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ProblemAccounts")]
        public string ProblemAccountJson { get; set; }

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
