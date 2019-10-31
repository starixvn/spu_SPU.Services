using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class ProcessCompletedCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromStagingTransID")]
        public int FromStagingTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToStagingTransID")]
        public int ToStagingTransId { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_ST_TransformData_UpdateCompletedProcess";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
