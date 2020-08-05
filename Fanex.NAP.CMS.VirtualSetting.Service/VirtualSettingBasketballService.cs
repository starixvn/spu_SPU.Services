namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VirtualSettingBasketballService : IVirtualSettingService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

        public VirtualSettingBasketballService(string siteIds, string superIds, DateTime adjustDate)
        {
            var model = new VirtualSetting
            {
                ServiceName = "Basketball",
                AdjustDate = adjustDate,
                SportGroupID = 1,
                SiteIds = siteIds,
                SuperIds = superIds,
                FailedUpdatePunterList = new List<Punter>()
            };

            var dataAccess = new VirtualSettingPunterDataAccess();

            _punterService = new VirtualSettingPunterService(model, dataAccess, true);
        }        

        public IVirtualSettingService AutoAdjustVirtualSettingsForPunters()
        {
            var customerList = _punterService.GetBasketballVRCustomers().ToList();

            var verifiedCustomers = _punterService.VerifyBasketballVRCustomers(string.Join(",", customerList));

            var customerChunks = Utilities.ChunkList(verifiedCustomers, AppSettings.PunterChunkSize).ToList();

            for(int i = 0; i < customerChunks.Count(); i ++)
            {
                _punterService.AdjustBasketballVR(string.Join(",", customerChunks[i]));
                PuntersUpdated += customerChunks[i].Count();
            }

            return this;
        }        
    }
}