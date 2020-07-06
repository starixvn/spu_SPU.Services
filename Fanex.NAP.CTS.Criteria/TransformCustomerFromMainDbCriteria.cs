using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class InsertNewCustomerCategoryCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIds")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_InsertNewCustomerCategory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIds);
        }
    }
}
