namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class VerifyVRCustomerCriteria : CriteriaBase
    {
        public string ListCustID { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_ProcessDataForBasketballVRSetting";
        }

        public override bool IsValid()
        {
            return true; 
        }
    }
}
