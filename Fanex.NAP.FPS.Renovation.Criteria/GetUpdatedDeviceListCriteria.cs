using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetUpdatedDeviceListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NoOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_TransformData_Device_Transaction_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
