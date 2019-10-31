using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class UpdateIsSyncGraphDBByAssociationIdCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AssociationJSON")]
        public string Association { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_UpdateIsSyncGraphDBByAssociationID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
