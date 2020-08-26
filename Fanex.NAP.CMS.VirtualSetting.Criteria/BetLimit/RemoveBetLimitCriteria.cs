
namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class RemoveBetLimitCriteria : CriteriaBase
    {
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_BLControl_PerformanceChecking_RemoveBetLimit";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDList);
        }
    }
}
