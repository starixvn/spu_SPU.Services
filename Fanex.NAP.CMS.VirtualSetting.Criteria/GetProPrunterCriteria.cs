
namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    public class GetProPrunterCriteria : CriteriaBase
    {        
        public string SiteIds { get; set; }

        public string SuperIds { get; set; }

        public double VirtualRateThreshold { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_Report_CustMngt_GetProPunters";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
