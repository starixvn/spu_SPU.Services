
namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class InsertBasketballVRCustomerCriteria : CriteriaBase
    {
        public string customerList { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(customerList);
        }
    }
}
