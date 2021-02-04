using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.RemoveEvidenceMultipleAccounts
{
    public class UpdateCustomerEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ProcessedIDList")]
        public string ProcessedIdList { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public  string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidenceQueue_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
