using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.RemoveEvidenceMultipleAccounts
{
    public class RollbackCustomerEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SessionId")]
        public long SessionId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidenceQueue_RollbackRemovedEvidences";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
