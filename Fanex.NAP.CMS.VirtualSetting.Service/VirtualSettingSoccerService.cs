namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;
    using System;

    public class VirtualSettingSoccerService : IVirtualSettingService
    {
        private IPunterService _punterService;

        public VirtualSettingSoccerService(string siteIds, string superIds, DateTime adjustDate)
        {
            var model = new VirtualSetting
            {
                ServiceName = "Soccer",
                AdjustDate = adjustDate,
                SportGroupID = 1,
                SiteIds = siteIds,
                SuperIds = superIds,
                VirtualRateThreshold = AppSetting.VirtualRateThresholdForWinning,
            };

            var dataAccess = new VirtualSettingPunterDataAccess();

            _punterService = new VirtualSettingPunterService(model, dataAccess);
        }

        public IVirtualSettingService AutoAdjustVirtualSettingsForPunters()
        {
            _punterService.GetPunterList()
                    .FilterPunterBaseOnCurrentVirtualRate()
                    .AdjustVirtualSettingForSuperProPunter(AppSetting.NewVirtualMultiplierForSuperProPunter)
                    .AdjustVirtualSettingForNormal(AppSetting.NewVirtualMultiplierForWinning);

            return this;
        }
        
    }
}