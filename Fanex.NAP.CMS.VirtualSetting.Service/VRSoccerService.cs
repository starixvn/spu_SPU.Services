namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;

    public class VRSoccerService : ICustomerSettingsService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

        public VRSoccerService(string siteIds, string superIds, DateTime adjustDate)
        {
            var model = new VirtualSetting
            {
                ServiceName = "Soccer",
                AdjustDate = adjustDate,
                SportGroupID = 1,
                SiteIds = siteIds,
                SuperIds = superIds,
                VirtualRateThreshold = AppSettings.VirtualRateThresholdForWinning,
                FailedUpdatePunterList = new List<Punter>()
            };

            var dataAccess = new VirtualSettingDataAccess();

            _punterService = new PunterService(model, dataAccess, true);
        }        

        public ICustomerSettingsService AutoAdjustVirtualSettings()
        {
            _punterService.GetWinningPunterSoccerList()
                    .FilterPunterBaseOnCurrentVirtualRate(1, false)
                    .AdjustVirtualSettingForSuperProPunter(AppSettings.NewVirtualMultiplierForSuperProPunter)
                    .AdjustVirtualSettingForNormal(AppSettings.NewVirtualMultiplierForWinning);

            PuntersUpdated = _punterService.CountAffected();

            return this;
        }

        public ICustomerSettingsService AutoAdjustBetLimit()
        {
            return this;
        }
    }
}