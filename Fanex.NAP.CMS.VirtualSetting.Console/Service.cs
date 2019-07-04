namespace Fanex.NAP.CMS.VirtualSetting.Console
{
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.CMS.VirtualSetting.Service;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;

    public class Service : BaseService
    {
        public void Execute()
        {
            var startTime = DateTime.Now;

            var puntersForSoccer = ExecuteForSoccer(AppSetting.SiteIds, AppSetting.SuperIds, startTime);

        }

        private IEnumerable<Punter> ExecuteForSoccer(string siteIds, string superIds, DateTime adjustDate)
        {
            var virtualSettingForSoccer = new VirtualSettingSoccerService(siteIds, superIds, adjustDate);

            return new List<Punter>();
        }

    }
}