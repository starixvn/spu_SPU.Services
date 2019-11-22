using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class GetAssociationDeviceByAccountsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_AssociationList")]
        public string AssociationList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetAssociationDeviceByAccounts";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
