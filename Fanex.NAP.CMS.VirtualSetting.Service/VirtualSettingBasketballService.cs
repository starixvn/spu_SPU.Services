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
            var CustomerList = _punterService.GetBasketballVRCustomers();

            var CustomerChunk = Utilities.ChunkList(CustomerList, AppSettings.PunterChunkSize).ToList();

            for(int i = 0; i < CustomerChunk.Count(); i ++)
            {
                _punterService.AdjustBasketballVR(CustomerChunk[i]);
                PuntersUpdated += CustomerChunk[i].Count();
            }

            return this;
        }        
    }
}