using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class MoveToTransaction07GetPackageCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformData_MoveTransaction_GetPackage";
        }

        public override bool IsValid()
        {
            return BatchSize != 0 || NumberOfBatch != 0;
        }
    }
}
