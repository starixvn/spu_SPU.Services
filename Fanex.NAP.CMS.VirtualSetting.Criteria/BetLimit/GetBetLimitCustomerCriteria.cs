namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class GetBetLimitCustomerCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "NAP_BLControl_GetPerformanceCheckList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
