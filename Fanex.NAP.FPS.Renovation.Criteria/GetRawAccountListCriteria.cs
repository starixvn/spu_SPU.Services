using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetRawAccountListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int NumberOfTickets { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCST_Account_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
