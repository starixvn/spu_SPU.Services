namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;

    public class VirtualSettingSoccerService : IVirtualSettingService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

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
                FailedUpdatePunterList = new List<Punter>(),
            };

            var dataAccess = new VirtualSettingPunterDataAccess();

            _punterService = new VirtualSettingPunterService(model, dataAccess, true);
        }        

        public IVirtualSettingService AutoAdjustVirtualSettingsForPunters()
        {
            _punterService.GetWinningPunterSoccerList()
                    .FilterPunterBaseOnCurrentVirtualRate(1, false)
                    .AdjustVirtualSettingForSuperProPunter(AppSetting.NewVirtualMultiplierForSuperProPunter)
                    .AdjustVirtualSettingForNormal(AppSetting.NewVirtualMultiplierForWinning);

            PuntersUpdated = _punterService.CountAffected();

            return this;
        }
        
    }
}