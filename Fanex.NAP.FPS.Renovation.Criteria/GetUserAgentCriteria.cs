using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetUserAgentCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_size")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_TransformData_UserAgent_GetNullBrowserOSList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
