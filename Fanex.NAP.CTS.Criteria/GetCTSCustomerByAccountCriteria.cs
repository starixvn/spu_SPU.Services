using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class GetCustomerByAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_AssociationList")]
        public string AssociationList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCTSCustomerByAccounts";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
