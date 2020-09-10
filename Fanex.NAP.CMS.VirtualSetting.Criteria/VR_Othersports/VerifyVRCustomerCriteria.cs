namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class VerifyVRCustomerCriteria : CriteriaBase
    {
        public string ListCustID { get; set; }

        public override string GetSettingKey()
        {
            return "Task_VRSetting_CheckAllSportsCustPerformance";
        }

        public override bool IsValid()
        {
            return true; 
        }
    }
}
