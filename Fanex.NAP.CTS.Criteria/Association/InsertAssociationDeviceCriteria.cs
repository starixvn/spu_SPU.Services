using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Association
{
    public class InsertAssociationDeviceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_Device_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
