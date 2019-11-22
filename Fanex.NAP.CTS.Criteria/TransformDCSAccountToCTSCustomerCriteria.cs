using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class TransformDCSAccountToCTSCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AccountList")]
        public string AccountJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_TransformDCSAccountToCTSCustomer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
