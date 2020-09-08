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
            var customerList = _punterService.GetSpecialSportBookVRAdjustmentCustomers(99).ToList();
            var customerListString = string.Join(",", customerList);

            LogService.Info("VR Setting - OtherSports - Raw Customer list: " + customerListString);

            var (passedPerfCheckCustomers, unpassedPerfCheckCustomers) = _punterService.VerifyVRAdjustmentCustomers(customerListString);

            AdjustVRPassedCustomers(passedPerfCheckCustomers, 99);
            UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 99);

            return this;
        }   

        public ICustomerSettingsService AutoAdjustVRForSpecialSportsbook()
        {
            var customerList = _punterService.GetSpecialSportBookVRAdjustmentCustomers(100).ToList();
            var customerListString = string.Join(",", customerList);

            LogService.Info("VR Setting - Special Licensee - Special SportBook - Raw Customer list: " + customerListString);

            var (passedPerfCheckCustomers, unpassedPerfCheckCustomers) = _punterService.VerifyVRAdjustmentCustomers(customerListString);

            AdjustVRPassedCustomers(passedPerfCheckCustomers, 100);
            UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 100);

            return this;
        }

        private void AdjustVRPassedCustomers(IEnumerable<int> customerList, int sportType)
        {
            var customerListString = string.Join(",", customerList);

            if (sportType == 99)
                LogService.Info("VR Setting - Special Licensee - OtherSports - Customers have passed Performance Check: " + customerListString);
            else
                LogService.Info("VR Setting - Special Licensee - Special SportBook - Customers have passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, AppSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                _punterService.UpdateVRFollowUpline(string.Join(",", customerChunks[i]), sportType);
                PuntersUpdated += customerChunks[i].Count();
            }
        }

        private void UpdateStatusUnpassedCustomers(IEnumerable<int> customerList, int sportType)
        {
            var customerListString = string.Join(",", customerList);

            if (sportType == 99)
                LogService.Info("VR Setting - Special Licensee - OtherSports - Customers have NOT passed Performance Check: " + customerListString);
            else
                LogService.Info("VR Setting - Special Licensee - Special SportBook - Customers have NOT passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, AppSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                _punterService.UpdateVRAutoAdjustmentStatus(string.Join(",", customerChunks[i]), sportType);
            }
        }

        public ICustomerSettingsService AutoAdjustBetLimit()
        {
            return this;
        }
    }
}