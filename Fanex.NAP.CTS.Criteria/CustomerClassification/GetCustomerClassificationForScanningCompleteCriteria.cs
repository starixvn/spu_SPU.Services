using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class GetCustomerClassificationForScanningCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportJson { get; set; }

        [SpParam(SqlParamName = "ip_ParentCategoryID")]
        public int ParentCategoryId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCustomerClassificationIntervalScan_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
