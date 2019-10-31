using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class UpdateDeviceIdCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson")]
        public string TransId { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_TransformData_Device_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
