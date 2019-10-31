using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetRawTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsProcessed")]
        public int IsProcessed { get; set; }

        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int NumberOfTickets { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_ST_TransformData_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
