namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.Logging;
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VROthersportsService : ICustomerSettingsService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

        public VROthersportsService(string siteIds, string superIds, DateTime adjustDate)
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

            var dataAccess = new VirtualSettingDataAccess();

            _punterService = new PunterService(model, dataAccess, true);
        }        

        public ICustomerSettingsService AutoAdjustVirtualSettings()
        {
            var customerList = _punterService.GetVRAdjustmentCustomers().ToList();
            var customerListString = string.Join(",", customerList);
            LogService.Info("OtherSports - VR Settings - Raw Customer list: " + customerListString);

            var verifiedCustomers = _punterService.VerifyVRAdjustmentCustomers(customerListString);

            AdjustVRPassedCustomers(verifiedCustomers.PassedPerfCheckCustomers);
            UpdateStatusUnpassedCustomers(verifiedCustomers.UnpassedPerfCheckCustomers);

            return this;
        }   
        
        public ICustomerSettingsService AutoAdjustBetLimit()
        {
            return this;
        }

        private void AdjustVRPassedCustomers(IEnumerable<int> customerList)
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

        private void UpdateStatusUnpassedCustomers(IEnumerable<int> customerList)
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