using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class InsertUserPermissionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserIds")]
        public string UserIds { get; set; }

        [SpParam(SqlParamName = "ip_FunctionName")]
        public string PermissionFuncName { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_InsertUserPermission";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}