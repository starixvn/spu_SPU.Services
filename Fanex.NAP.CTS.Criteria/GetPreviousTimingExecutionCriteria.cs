using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class GetPreviousTimingExecutionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParamID")]
        public int ParamID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetSystemParameterById";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}