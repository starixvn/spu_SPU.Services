using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class UpdateUserAgentCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserAgentJson")]
        public string UserAgentJson { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_TransformData_UserAgent_UpdateOSBrowser";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
