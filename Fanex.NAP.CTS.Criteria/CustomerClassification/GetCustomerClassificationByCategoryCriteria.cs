using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class GetCustomerClassificationByCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CategoryIDList")]
        public string CategoryIdList { get; set; }

        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCustomerClassificationByCategory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CategoryIdList);
        }
    }
}
