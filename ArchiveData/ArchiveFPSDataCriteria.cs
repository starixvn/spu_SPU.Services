using Fanex.Data;
using Fanex.Data.Repository;

namespace ArchiveData
{
    public class ArchiveFPSDataCriteria : NonQueryCommand
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
