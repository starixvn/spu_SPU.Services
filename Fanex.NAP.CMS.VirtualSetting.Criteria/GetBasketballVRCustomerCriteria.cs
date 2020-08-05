namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class GetBasketballVRCustomerCriteria : CriteriaBase
    {
        public int Sporttype { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_GetAdjustmentCustomerList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
