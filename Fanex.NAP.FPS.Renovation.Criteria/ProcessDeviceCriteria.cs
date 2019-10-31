using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class ProcessDeviceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromStagingTransID")]
        public int FromStagingTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToStagingTransID")]
        public int ToStagingTransId { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_TransformData_Device";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
