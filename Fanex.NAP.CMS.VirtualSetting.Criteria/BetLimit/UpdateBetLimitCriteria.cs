
namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateBetLimitCriteria : CriteriaBase
    {
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_BLControl_PerformanceChecking_ReduceBetLimit";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDList);
        }
    }
}
