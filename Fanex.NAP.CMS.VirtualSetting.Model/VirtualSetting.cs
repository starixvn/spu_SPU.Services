namespace Fanex.NAP.CMS.VirtualSetting.Model
{
    using System;
    using System.Collections.Generic;

    public class VirtualSetting
    {
        public string ServiceName { get; set; }

        public DateTime AdjustDate { get; set; }

        public byte SportGroupID { get; set; }

        public string SiteIds { get; set; }

        public string SuperIds { get; set; }

        public double VirtualRateThreshold { get; set; }

        public bool IsSuccessful { get; set; }

        public List<Punter> FailedUpdatePunterList { get; set; }

        public PunterEqualityComparer PunterEqualityComparer { get; set; }
    }
}