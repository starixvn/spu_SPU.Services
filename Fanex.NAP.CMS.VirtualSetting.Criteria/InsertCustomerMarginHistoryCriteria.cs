namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class InsertCustomerMarginHistoryCriteria : CriteriaBase
    {
        public string CustomerMarginDoc { get; set; }

        public DateTime AdjustDate { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_UserLog_InsertCustomerMarginHistory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerMarginDoc);
        }
    }
}
