using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.CTS.Criteria
{
    public class GetRobotUserCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public string CreditSite { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_RobotListSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
