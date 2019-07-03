namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class GetCustomerMarginHistoryCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public byte SportGroupID { get; set; }

        public int NumOfMonth { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_Report_CustMngt_GetCustomerMarginHistory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIds) && NumOfMonth > 0;
        }
    }
}
