using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetIPTableLogLatestUpdatedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FPS_DC_GetLatestUpdatedDate_IPRange";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
