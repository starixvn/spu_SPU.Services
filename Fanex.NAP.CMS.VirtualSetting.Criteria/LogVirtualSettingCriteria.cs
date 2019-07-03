namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class LogVirtualSettingCriteria : CriteriaBase
    {
        public string PunterVRSettingsDoc { get; set; }
        
        public int CreatorId { get; set; }

        public string ServiceName { get; set; }

        public DateTime AdjustDate { get; set; } 

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
       
        public override string GetSettingKey()
        {
            return "NAP_UserLog_UpdateVirtualRateSettings";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
