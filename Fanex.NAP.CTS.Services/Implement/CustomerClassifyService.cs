namespace Fanex.NAP.CTS.Services
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.Core.Constants;

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

        public void RescanCustomerClassify(List<int> custIds, bool isRescanPA, bool isPushCCToSoon88, FLOWTYPERESCANCC flowTypeRescanCC)
        {
            var custIdList = new List<int>();

            //Prepare SMA CustId to sync CC
            var roleCusomters = GetAgentCusts(custIds);
            custIdList.AddRange(roleCusomters);

            // Re-Sync Customer Evidence to category in CTS
            var custIdCCs = SyncCustomerEvidenceToCategory(custIds, flowTypeRescanCC);
            custIdList.AddRange(custIdCCs);

            if (isRescanPA)
            {
                // Re-Insert Problem Account in CTS
                custIdCCs = InsertProblemAccount(custIds, flowTypeRescanCC);
                custIdList.AddRange(custIdCCs);
            }

            var custIdsToSyncCC = custIdList.Distinct().ToList();

            // Push Customer class to MainDB, Soon88, ...
            if(custIdsToSyncCC!=null && custIdsToSyncCC.Any())
            {
                SyncCustomerClassToExternal(custIdsToSyncCC, isPushCCToSoon88);
            } 

            // Check and rescan normal account
            if (custIds != null && custIds.Any())
            {
                var newCustIds = custIds.Where(x => !custIdCCs.Contains(x)).ToList();
                RescanNormalAccount(newCustIds);
            }
        }

        private List<int> SyncCustomerEvidenceToCategory(List<int> custIds, FLOWTYPERESCANCC flowTypeRescanCC)
        {
            var custIdsToSyncCC = new List<int>();

            var existedCustEvidences = GetExistedCustomerEvidences(string.Join(",", custIds)).ToList();

            var isWriteLogRescan = flowTypeRescanCC != FLOWTYPERESCANCC.NEWMEMBER;

            // Re-Sync Customer Evidence to category in CTS
            var result = _customerEvidenceService.SyncCustomerEvidenceToCategory(existedCustEvidences, isWriteLogRescan);

            if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null && result.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result.CustIds);
            }

            return custIdsToSyncCC;
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

        private List<int> InsertProblemAccount(List<int> custIds, FLOWTYPERESCANCC flowTypeRescanCC)
        {
            var custIdsToSyncCC = new List<int>();

            var existedProblemAccounts = GetExistedProblemAccounts(string.Join(",", custIds)).ToList();

            var custs = InsertProblemAccountToCTS(existedProblemAccounts, flowTypeRescanCC);

            if (custs != null && custs.Any())
            {
                custIdsToSyncCC = custs;
            }

            return custIdsToSyncCC;
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

            return problemAccounts.Distinct();
        }

        private List<int> InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts, FLOWTYPERESCANCC flowTypeRescanCC)
        {
            var inputData = problemAccounts.Select(a => new InsertProblemAccountsModel
            {
                CustId = a.CustID,
                CategoryId = a.ReasonID
            }).ToList();

            var problemAccountsChunk = inputData.Chunk(AppSetting.UpdateInternalChunkSize);

            var custIds = new List<int>();

            var isWriteLogRescan = flowTypeRescanCC != FLOWTYPERESCANCC.NEWMEMBER;

            foreach (var problemAccs in problemAccountsChunk)
            {
                var result = _dataCenterService.InsertProblemAccounts(problemAccs, isWriteLogRescan);

                custIds.AddRange(result.CustIds);
            }

            return custIds;
        }

        private void SyncCustomerClassToExternal(List<int> custIdsToSyncCC, bool isPushCCToSoon88)
        {
            var custIdsChunk = custIdsToSyncCC.Distinct().Chunk(AppSettings.PushCCToExternalChunkSize);

            foreach (var custids in custIdsChunk)
            {
                var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",", custids));

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    bool isSyncMainSuccessfully = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, AppSettings.RetryTime);

                    if (isSyncMainSuccessfully)
                        _dataCenterService.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));

                    if (isPushCCToSoon88)
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

            var custIdsChunk = customers.Chunk(AppSettings.InternalChunkSize).ToList();

            foreach (var custIds in custIdsChunk)
            {
                var custIdsToBeRescaned = string.Join(",", custIds);

                _dataCenterService.ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned, false);
            }
        }

        private List<int> GetAgentCusts(List<int> custIds)
        {
            var agentIds = new List<int>();

            if (custIds != null && custIds.Any())
            {
                var data = _dataCenterService.GetRoleCustomer(string.Join(",", custIds)).ToList();

                if (data != null && data.Any())
                {
                    agentIds = data.Where(c => c.RoleID == (int)ROLE_CUSTOMER.AGENT
                                            || c.RoleID == (int)ROLE_CUSTOMER.MASTER
                                            || c.RoleID == (int)ROLE_CUSTOMER.SUPER)
                               .Select(c => c.CustID).ToList();
                }
            }

            return agentIds;
        }
    }
}
