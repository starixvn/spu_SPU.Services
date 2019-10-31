using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class IpInfoCriteria : CriteriaBase
    {
        public string IpAddress { get; set; }

        public override string GetSettingKey()
        {
            return "GetIPDetail";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
