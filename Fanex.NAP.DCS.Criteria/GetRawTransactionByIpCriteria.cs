using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Criteria
{
    public class GetRawTransactionByIpCriteria : CriteriaBase
    {

        [SpParam(SqlParamName = "ip_NoOfTransaction")]
        public int NumberOfTrans { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCTS_TransactionIP_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
