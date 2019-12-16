using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.DCS.Criteria
{
    public class GetRawUnsucessAccountListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfAccount")]
        public int NumberOfAccount { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCTS_Account_Unsuccess_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
