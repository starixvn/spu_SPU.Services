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

    public class RescanCustomerClassificationService : IRescanCustomerClassificationService
    {
        private readonly IDataCenterService _dataCenterService;
        private readonly IPushDataToExternalService _pushDataToExternalService;

        public RescanCustomerClassificationService()
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
                    _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, AppSettings.RetryTime);

                    _dataCenterService.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));

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

            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);

            var custIdsChunk = customers.Chunk(batchSize).ToList();

            foreach (var custIds in custIdsChunk)
            {
                var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",",custIds));

                var custIdsToBeRescaned = string.Join(",", lastestClassifications.Select(cust => cust.CustId));

                _dataCenterService.ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned);
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
