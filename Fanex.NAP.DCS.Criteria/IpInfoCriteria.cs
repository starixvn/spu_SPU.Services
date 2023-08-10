using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Criteria
{
    public class IpInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "IPs")]
        public string IpAddressList { get; set; }

        public override string GetSettingKey()
        {
            return "GetLocationByIPList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
