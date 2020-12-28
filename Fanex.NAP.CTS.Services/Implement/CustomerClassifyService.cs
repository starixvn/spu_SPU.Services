namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;    
    using System.Collections.Generic;    
    using System.Linq;

    public class CustomerClassifyService : ICustomerClassifyService
    {
        private readonly IDataCenterService _dataCenterService;
        private readonly IPushDataToExternalService _pushDataToExternalService;
        private readonly ICustomerEvidenceService _customerEvidenceService;

        public CustomerClassifyService()
        {
            _dataCenterService = new DataCenterService();
            _pushDataToExternalService = new PushDataToExternalService();
            _customerEvidenceService = new CustomerEvidenceService();
        }

        public void RescanCustomerClassification(List<int> custIds)
        {
            // Check and rescan problem account
            var existedCustEvidences = GetExistedCustomerEvidences(string.Join(",", custIds)).ToList();
            var existedProblemAccounts = GetExistedProblemAccounts(string.Join(",", custIds)).ToList();

            var isRescanPA = (existedCustEvidences != null && existedCustEvidences.Any()) || (existedProblemAccounts != null && existedProblemAccounts.Any());

            if (isRescanPA)
            {
                RescanProblemAccount(existedProblemAccounts, existedCustEvidences);

                custIds.RemoveAll(item => existedProblemAccounts.Select(c => c.CustID).Contains(item));
                custIds.RemoveAll(item => existedCustEvidences.Select(c => c.CustId).Contains(item));
            }

            // Check and rescan normal account
            if (custIds != null && custIds.Any())
            {
                RescanNormalAccount(custIds);
            }
        }

        private IEnumerable<CustEvidenceModel> GetExistedCustomerEvidences(string custIds)
        {
            var existedEvidences = new List<CustEvidenceModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return existedEvidences;
            }

            existedEvidences = _dataCenterService.GetExistedEvidences(custIds).ToList();

            return existedEvidences ?? new List<CustEvidenceModel>();
        }

        private IEnumerable<MultipleProblemAccountsModel> GetExistedProblemAccounts(string custIds)
        {
            var problemAccounts = new List<MultipleProblemAccountsModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return problemAccounts;
            }

            var problemAccountsNAPCM = _dataCenterService.GetProblemAccounts(custIds, null).ToList();
            var problemAccountsCN88 = _dataCenterService.GetProblemAccounts_CN88(custIds, null).ToList();

            if (problemAccountsNAPCM != null && problemAccountsNAPCM.Any())
            {
                problemAccounts.AddRange(problemAccountsNAPCM);
            }

            if (problemAccountsCN88 != null && problemAccountsCN88.Any())
            {
                problemAccounts.AddRange(problemAccountsCN88);
            }

            return problemAccounts;
        }

        private void RescanProblemAccount(List<MultipleProblemAccountsModel> problemAccounts, List<CustEvidenceModel> existedEvidences)
        {            
            var custIdsToSyncCC = new List<int>();

            // Re-Sync Customer Evidence to category in CTS
            var result = _customerEvidenceService.SyncCustomerEvidenceToCategory(existedEvidences);

            if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null && result.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result.CustIds);
            }

            // Re-Insert Problem Account in CTS
            result = InsertProblemAccountToCTS(problemAccounts);

            if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null && result.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result.CustIds);
            }

            var custIdsChunk = custIdsToSyncCC.Distinct().Chunk(AppSettings.BatchSize);

            foreach (var custids in custIdsChunk)
            {
                var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",", custids));

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    bool isSyncMainSuccessfully = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, AppSettings.RetryTime);

                    if (isSyncMainSuccessfully)
                    {
                        _dataCenterService.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));
                    }

                    _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                }
            }
        }

        private void RescanNormalAccount(List<int> customers)
        {
            if (customers == null || !customers.Any())
            {
                return;
            }

            var custIdsChunk = customers.Chunk(AppSettings.BatchSize).ToList();

            foreach (var custIds in custIdsChunk)
            {
                var custIdsToBeRescaned = string.Join(",", custIds);

                _dataCenterService.ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned, false);
            }
        }               

        private InsertCategoryResultModel InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
            var result = problemAccounts.Select(a => new InsertProblemAccountsModel
            {
                CustId = a.CustID,
                CategoryId = a.ReasonID
            }).ToList();

            return _dataCenterService.InsertProblemAccounts(result);
        }

    }
}
