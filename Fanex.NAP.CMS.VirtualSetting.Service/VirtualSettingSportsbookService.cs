namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;

    public class VirtualSettingSportsbookService : IVirtualSettingService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

        public VirtualSettingSportsbookService(string siteIds, string superIds, DateTime adjustDate)
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

            var dataAccess = new VirtualSettingPunterDataAccess();

            _punterService = new VirtualSettingPunterService(model, dataAccess);
        }

        public IVirtualSettingService AutoAdjustVirtualSettingsForPunters()
        {
            _punterService.GetWinningPunterList()
                .FilterPunterBaseOnCurrentVirtualRate(0, true)
                .AdjustVirtualSettingForNormal(AppSettings.NewVirtualMultiplierForWinning);

            PuntersUpdated = _punterService.CountAffected();

            return this;
        }
    }
}