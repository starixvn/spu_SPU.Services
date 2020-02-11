using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.DCS.Criteria
{
    public class GetRawUnsuccessTransactionIPListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfTransaction")]
        public int NumberOfTrans { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCTS_TransactionIP_Unsuccess_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
