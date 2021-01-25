using Fanex.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class GetAssociationProblemAccountForNewMemberClassificationCriteria : Data.Repository.CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListCustID")]
        public string CustIdList { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_NewMember_GetAssociationPA";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
