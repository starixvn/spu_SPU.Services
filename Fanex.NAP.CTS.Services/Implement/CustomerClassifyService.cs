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

        public bool RescanCustomerClassification(List<int> custIds)
        {
            var isSuccess = true;

            // Check and rescan problem account
            var existedEvidences = CheckExistedEvidenceToRescanPA(string.Join(",", custIds)).ToList();
            var problemAccounts = CheckToReScanPAFromNapCM(string.Join(",", custIds)).ToList();

            if ((problemAccounts != null && problemAccounts.Any()) || (existedEvidences != null && existedEvidences.Any()))
            {
                RescanProblemAccount(problemAccounts, existedEvidences);

                custIds.RemoveAll(item => problemAccounts.Select(c => c.CustID).Contains(item));
                custIds.RemoveAll(item => existedEvidences.Select(c => c.CustId).Contains(item));
            }

            // Check and rescan normal account
            if (custIds != null && custIds.Any())
            {
                RescanNormalAccount(custIds);
            }

            return isSuccess;
        }

        private void RescanProblemAccount(List<MultipleProblemAccountsModel> problemAccounts, List<CustEvidenceModel> existedEvidences)
        {
            var result1 = _customerEvidenceService.InsertCustEvidenceToCategory(existedEvidences);

            var result2 = InsertProblemAccountToCTS(problemAccounts);

            var custIdsToSyncCC = new List<int>();

            if (result1.CustIds != null && result1.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result1.CustIds);
            }
            else if (result2.CustIds != null && result2.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result2.CustIds);
            }

            if (custIdsToSyncCC == null || !custIdsToSyncCC.Any())
            {
                return;
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

        private bool RescanNormalAccount(List<int> customers)
        {
            if (customers == null || !customers.Any())
            {
                return true;
            }

            var custIdsChunk = customers.Chunk(AppSettings.BatchSize).ToList();

            foreach (var custIds in custIdsChunk)
            {
                var custIdsToBeRescaned = string.Join(",", custIds);

                _dataCenterService.ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned, false);
            }

            return true;
        }

        private IEnumerable<MultipleProblemAccountsModel> CheckToReScanPAFromNapCM(string custIds)
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

        private IEnumerable<CustEvidenceModel> CheckExistedEvidenceToRescanPA(string custIds)
        {
            var existedEvidences = new List<CustEvidenceModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return existedEvidences;
            }

            existedEvidences = _dataCenterService.GetExistedEvidences(custIds).ToList();

            return existedEvidences ?? new List<CustEvidenceModel>();
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
