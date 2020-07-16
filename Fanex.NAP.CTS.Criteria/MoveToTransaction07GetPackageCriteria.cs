using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class MoveToTransaction07Criteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransIDList")]
        public string TransIdList { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformData_MoveTransaction";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(TransIdList);
        }
    }
}
