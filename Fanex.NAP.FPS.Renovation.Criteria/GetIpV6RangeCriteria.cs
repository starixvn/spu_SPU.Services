using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class GetIpV6RangeCriteria : CriteriaBase
    {
        public int FromId { get; set; }

        public int ToId { get; set; }

        public override string GetSettingKey()
        {
            return "GetAllIPv62Location";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
