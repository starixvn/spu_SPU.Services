namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.Logging;
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
                ServiceName = "OtherSports",
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
            var customerList = _punterService.GetVRAdjustmentCustomers().ToList();
            var customerListString = string.Join(",", customerList);
            LogService.Info("OtherSports - VR Settings - Raw Customer list: " + customerListString);

            var verifiedCustomers = _punterService.VerifyVRAdjustmentCustomers(customerListString);

            AdjustPassedPerformanceCustomersVR(verifiedCustomers.PassedPerfCheckCustomers);
            UpdateUnpassedPerformanceCustomersVRStatus(verifiedCustomers.UnpassedPerfCheckCustomers);

            return this;
        }        

        private void AdjustPassedPerformanceCustomersVR(IEnumerable<int> customerList)
        {
            var customerListString = string.Join(",", customerList);
            LogService.Info("OtherSports - VR Settings - Customers have passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, AppSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                _punterService.UpdateVRFollowUpline(string.Join(",", customerChunks[i]));
                PuntersUpdated += customerChunks[i].Count();
            }
        }

        private void UpdateUnpassedPerformanceCustomersVRStatus(IEnumerable<int> customerList)
        {
            var customerListString = string.Join(",", customerList);
            LogService.Info("OtherSports - VR Settings - Customers have NOT passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, AppSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                _punterService.UpdateVRAutoAdjustmentStatus(string.Join(",", customerChunks[i]));
            }
        }
    }
}