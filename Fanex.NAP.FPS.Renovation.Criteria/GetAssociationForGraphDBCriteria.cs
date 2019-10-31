using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetAssociationForGraphDBCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "ip_SyncGraphDB")]
        public int SyncGraphDB { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_GetAssociationForGraphDB";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
