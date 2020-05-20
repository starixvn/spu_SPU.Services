using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class ArchiveTransactionAfterTransformingCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int NoOfTickets { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformData_MoveTransaction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
