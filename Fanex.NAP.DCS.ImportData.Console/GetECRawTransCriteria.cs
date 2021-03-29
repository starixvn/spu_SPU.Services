using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.DCS.ImportData.Console
{
    public class GetECRawTransCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int NumberOfTickets { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Ecup_GetTransToQueue";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
