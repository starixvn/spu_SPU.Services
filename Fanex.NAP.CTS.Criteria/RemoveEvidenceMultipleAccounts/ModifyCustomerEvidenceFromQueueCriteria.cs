using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.RemoveEvidenceMultipleAccounts
{
    public class ModifyCustomerEvidenceFromQueueCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustEvidence")]
        public string CustomerEvidence { get; set; }

        [SpParam(SqlParamName = "ip_ActionID")]
        public int ActionId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }


        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidenceQueue_TakeAction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
