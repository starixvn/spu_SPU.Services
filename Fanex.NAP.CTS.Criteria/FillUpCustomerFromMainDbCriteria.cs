using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class FillUpCustomerFromMainDbCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_HealingCTSCustomer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
