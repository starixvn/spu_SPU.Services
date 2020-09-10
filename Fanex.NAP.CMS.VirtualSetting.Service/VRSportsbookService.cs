namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;

    public class VRSportsbookService : ICustomerSettingsService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

        public VRSportsbookService(string siteIds, string superIds, DateTime adjustDate)
        {
            var model = new VirtualSetting
            {
                ServiceName = "Sportsbook",
                AdjustDate = adjustDate,
                SportGroupID = 0,
                SiteIds = siteIds,
                SuperIds = superIds,                
                VirtualRateThreshold = AppSettings.VirtualRateThresholdForWinning,
                FailedUpdatePunterList = new List<Punter>(),
            };

            var dataAccess = new VirtualSettingDataAccess();

            _punterService = new PunterService(model, dataAccess);
        }

        public ICustomerSettingsService AutoAdjustVirtualSettings()
        {
            _punterService.GetWinningPunterList()
                .FilterPunterBaseOnCurrentVirtualRate(0, true)
                .AdjustVirtualSettingForNormal(AppSettings.NewVirtualMultiplierForWinning);

            PuntersUpdated = _punterService.CountAffected();

            return this;
        }

        public ICustomerSettingsService AutoAdjustBetLimit()
        {
            return this;
        }

        public ICustomerSettingsService AutoAdjustVRForSpecialSportsbook()
        {
            return this;
        }
    }
}