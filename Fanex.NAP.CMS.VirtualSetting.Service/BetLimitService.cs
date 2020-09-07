namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.Logging;
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BetLimitService : ICustomerSettingsService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

        public BetLimitService(string siteIds, string superIds, DateTime adjustDate)
        {
            var model = new VirtualSetting
            {
                ServiceName = "CustomerSettings_BetLimit",
                AdjustDate = adjustDate,
                SportGroupID = 1,
                SiteIds = siteIds,
                SuperIds = superIds,
                FailedUpdatePunterList = new List<Punter>()
            };

            var dataAccess = new VirtualSettingDataAccess();

            _punterService = new PunterService(model, dataAccess, true);
        }        

        public ICustomerSettingsService AutoAdjustBetLimit()
        {
            var customerList = _punterService.GetBetLimitCustomers().ToList();
            var customerListString = string.Join(",", customerList);
            LogService.Info("Customer Settings - Auto Update Bet Limit - Raw Customer list: " + customerListString);

            var (removeBetLimitCustomers, updateBetLimitCustomer) = _punterService.VerifyBetLimitCustomers(customerListString);

            RemoveBetLimit(removeBetLimitCustomers);
            UpdateBetLimit(updateBetLimitCustomer);

            return this;
        }        

        public ICustomerSettingsService AutoAdjustVirtualSettings()
        {
            return this;
        }

        private void UpdateBetLimit(IEnumerable<int> customerList)
        {
            var customerListString = string.Join(",", customerList);
            LogService.Info("Customer Settings - Auto Update Bet Limit - Customers have passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, AppSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                _punterService.UpdateBetLimit(string.Join(",", customerChunks[i]));
                PuntersUpdated += customerChunks[i].Count();
            }
        }

        private void RemoveBetLimit(IEnumerable<int> customerList)
        {
            var customerListString = string.Join(",", customerList);
            LogService.Info("Customer Settings - Auto Update Bet Limit - Customers have NOT passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, AppSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                _punterService.RemoveBetLimit(string.Join(",", customerChunks[i]));
            }
        }

        public ICustomerSettingsService AutoAdjustVRForSpecialSportBook()
        {
            return this;
        }
    }
}