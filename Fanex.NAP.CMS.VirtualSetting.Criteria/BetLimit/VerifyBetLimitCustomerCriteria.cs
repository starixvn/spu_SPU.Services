namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class VerifyBetLimitCustomerCriteria : CriteriaBase
    {
        public string ListCustID { get; set; }

        public override string GetSettingKey()
        {
            return "Task_BetLimit_CheckCustPerformance";
        }

        public override bool IsValid()
        {
            return true; 
        }
    }
}
