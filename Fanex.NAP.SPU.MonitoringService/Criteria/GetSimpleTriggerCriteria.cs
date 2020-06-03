namespace Fanex.NAP.SPU.MonitoringService.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;

    public class GetSimpleTriggerCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_GetSimpleTrigger";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
