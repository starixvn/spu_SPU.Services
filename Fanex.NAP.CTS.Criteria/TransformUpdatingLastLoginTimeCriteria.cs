using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class TransformUpdatingLastLoginTimeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Batch")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_TransformUpdateLastLoginTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
