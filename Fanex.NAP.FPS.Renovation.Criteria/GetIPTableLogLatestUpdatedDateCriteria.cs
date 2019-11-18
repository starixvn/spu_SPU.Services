using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Criteria
{
    public class GetIPTableLogLatestUpdatedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_GetLatestUpdatedDate_IPRange";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
