using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class GetCustomersForUpdateUsername2Criteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public int MaxCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_IsUpdateCustsub")]
        public bool IsUpdateCustSub { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCustomerForUpdateUsername2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
