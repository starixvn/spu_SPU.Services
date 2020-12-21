namespace Fanex.NAP.CTS.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RescanCustomerClassifyService : IRescanCustomerClassifyService
    {
        private readonly IDataCenterService _dataCenterService;
        private readonly IPushDataToExternalService _pushDataToExternalService;

        public RescanCustomerClassifyService()
        {
            _dataCenterService = new DataCenterService();
            _pushDataToExternalService = new PushDataToExternalService();
        }

        public bool RescanCustomerClassification(List<int> custIds)
        {
            var isSuccess = true;

            // Check and rescan problem account
            var problemAccounts = CheckToReScanPA(string.Join(",", custIds.Select(c => c))).ToList();

            if (problemAccounts != null && problemAccounts.Any())
            {
                RescanProblemAccount(problemAccounts);

                custIds.RemoveAll(item => problemAccounts.Select(c => c.CustID).Contains(item));
            }

            // Check and rescan normal account
            if (custIds != null && custIds.Any())
            {
                RescanNormalAccount(custIds);
            }

            return isSuccess;
        }

        private void RescanProblemAccount(List<MultipleProblemAccountsModel> problemAccounts)
        {
            if (problemAccounts == null && !problemAccounts.Any())
            {
                return;
            }

            var latestCustIds = InsertProblemAccountToCTS(problemAccounts);

            if (latestCustIds == null && !latestCustIds.Any())
            {
                return;
            }

            var latestCustIdsChunk = latestCustIds.Chunk(AppSettings.BatchSize);

            foreach (var custids in latestCustIdsChunk)
            {
                var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",",custids));

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    bool isSyncMainSuccessfully = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, AppSettings.RetryTime);

                    if(isSyncMainSuccessfully)
                    {
                        _dataCenterService.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));
                    }

                    _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                }
            }
        }

        private bool RescanNormalAccount(List<int> customers)
        {
            if (customers == null || !customers.Any())
            {
                return true;
            }

            var custIdsChunk = customers.Chunk(AppSettings.BatchSize).ToList();

            foreach (var custIds in custIdsChunk)
            {
                var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",",custIds));

                var custIdsToBeRescaned = string.Join(",", lastestClassifications.Select(cust => cust.CustId));

                _dataCenterService.ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned, false);
            }

            return true;
        }

        private IEnumerable<MultipleProblemAccountsModel> CheckToReScanPA(string custIds)
        {
            var problemAccounts = new List<MultipleProblemAccountsModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return problemAccounts;
            }

            problemAccounts = _dataCenterService.GetProblemAccounts(custIds, null).ToList();

            problemAccounts.AddRange(_dataCenterService.GetProblemAccounts_CN88(custIds, null).ToList());

            return problemAccounts;
        }

        private IEnumerable<int> InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
            var result = problemAccounts.Select(a => new InsertProblemAccountsModel
            {
                CustId = a.CustID,
                CategoryId = a.ReasonID
            });

            var jsonData = JsonConvert.SerializeObject(result);

            return _dataCenterService.InsertProblemAccounts(jsonData);
        }
    }
}
