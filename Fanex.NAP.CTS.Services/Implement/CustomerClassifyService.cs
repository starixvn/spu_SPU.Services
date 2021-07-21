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
        private readonly bool _isWriteLogInfo;

        public CustomerClassifyService()
        {
            _dataCenterService = new DataCenterService();
            _pushDataToExternalService = new PushDataToExternalService();
            _customerEvidenceService = new CustomerEvidenceService();
            _isWriteLogInfo = AppSettings.IsWriteLogInfo;
        }

        public void RescanCustomerClassify(List<int> custIds, bool isCheckPAFromNAP, bool isPushCCToSoon88, FLOWTYPERESCANCC flowTypeRescanCC)
        {
            var custIdList = new List<int>();

            //Rescan PA - Get SMA CustId
            var roleCusomters = GetAgentCusts(custIds);
            custIdList.AddRange(roleCusomters);

            // Rescan PA - Get PA From Evidence in CTS
            var problemAccountsFromEvi = SyncCustomerEvidenceToCategory(custIds, flowTypeRescanCC);
            custIdList.AddRange(problemAccountsFromEvi);

            // Rescan PA - Get PA From RobotUser & NAP PA
            var problemAccounts = GetProblemAccounts(custIds, isCheckPAFromNAP);

            if (problemAccounts!= null && problemAccounts.Any())
            {
                custIdList.AddRange(InsertProblemAccount(problemAccounts, flowTypeRescanCC));
            }

            //Rescan PA - Push CC & CN88
            var custIdsToSyncCC = custIdList.Distinct().ToList();

            if (custIdsToSyncCC != null && custIdsToSyncCC.Any())
            {
                SyncCustomerClassToExternal(custIdsToSyncCC, isPushCCToSoon88);
            }

            // Rescan Normal 
            custIds.RemoveAll(x => custIdsToSyncCC.Contains(x));

            if (_isWriteLogInfo)
                LogService.Info($"Normal Rescan - with data: {JsonConvert.SerializeObject(custIds)}");

            if (custIds != null && custIds.Any())
            {
                RescanNormalAccount(custIds, flowTypeRescanCC);
            }
        }

        private List<int> SyncCustomerEvidenceToCategory(List<int> custIds, FLOWTYPERESCANCC flowTypeRescanCC)
        {
            var custIdsToSyncCC = new List<int>();

            var existedCustEvidences = GetExistedCustomerEvidences(string.Join(",", custIds)).ToList();

            var isWriteLogRescan = flowTypeRescanCC != FLOWTYPERESCANCC.NEWMEMBER;

            var result = _customerEvidenceService.SyncCustomerEvidenceToCategory(existedCustEvidences, isWriteLogRescan);

            if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null && result.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result.CustIds);
            }

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Get Evidence Custs - with data: {JsonConvert.SerializeObject(custIdsToSyncCC)}");

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

        private List<MultipleProblemAccountsModel> GetProblemAccounts(List<int> custIds, bool isCheckPAFromNAP)
        {
            var problemAccounts = new List<MultipleProblemAccountsModel>();

            if (custIds != null && custIds.Any())
            {
                //PA - RobotUser
                var robotUsers = SyncRobotUserToUpdateCategory(custIds);

                if (robotUsers != null && robotUsers.Any())
                {
                    robotUsers.ForEach(c =>
                    {
                        problemAccounts.Add(new MultipleProblemAccountsModel()
                        {
                            CustID = c,
                            ReasonID = (int)CATEGORY.ROBOT
                        });
                    });
                }

                if (_isWriteLogInfo)
                    LogService.Info($"PA Rescan - Get RobotUsers - with data: {JsonConvert.SerializeObject(problemAccounts)}");

                //PA - NAP PA
                if (isCheckPAFromNAP)
                {
                    var existedProblemAccounts = GetProblemAccountFromNAPPA(string.Join(",", custIds)).ToList();
                    problemAccounts.AddRange(existedProblemAccounts);
                    
                    if (_isWriteLogInfo)
                        LogService.Info($"PA Rescan - Get PA Custs - with data: {JsonConvert.SerializeObject(existedProblemAccounts)}");
                }
            }

            return problemAccounts;
        }

        private List<int> InsertProblemAccount(List<MultipleProblemAccountsModel> problemAccounts, FLOWTYPERESCANCC flowTypeRescanCC)
        {
            var custIdsToSyncCC = new List<int>();

            if (problemAccounts != null && problemAccounts.Any())
            { 
                custIdsToSyncCC = InsertProblemAccountToCTS(problemAccounts, flowTypeRescanCC);
            }

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Get PA Custs (Robot & NAPPA) - with data: {JsonConvert.SerializeObject(custIdsToSyncCC)}");

            return custIdsToSyncCC;
        }

        private IEnumerable<MultipleProblemAccountsModel> GetProblemAccountFromNAPPA(string custIds)
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
            var custIds = new List<int>();

            if(problemAccounts!=null && problemAccounts.Any())
            {
                var problemAccountsChunk = problemAccounts.Chunk(AppSetting.UpdateInternalChunkSize);

                var isWriteLogRescan = flowTypeRescanCC != FLOWTYPERESCANCC.NEWMEMBER;

                foreach (var problemAccs in problemAccountsChunk)
                {
                    var result = _dataCenterService.InsertProblemAccounts(problemAccs, isWriteLogRescan);

                    custIds.AddRange(result.CustIds);
                }
            }    

            return custIds;
        }

        private List<long> SyncRobotUserToUpdateCategory(List<int> custIds)
        {
            var robotUser = new List<long>();

            var custs = _dataCenterService.GetCustByRobotSiteRule(string.Join(",", custIds)).ToList();

            var getRobotUser = _dataCenterService.GetExistedRobotUser(string.Join(",", custs));

            if (!string.IsNullOrEmpty(getRobotUser))
            {
                robotUser = getRobotUser.Split(',').Select(long.Parse).ToList();
            }

            return robotUser;
        }

        private void SyncCustomerClassToExternal(List<int> custIdsToSyncCC, bool isPushCCToSoon88)
        {
            var custIdsChunk = custIdsToSyncCC.Distinct().Chunk(AppSettings.PushCCToExternalChunkSize);

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - Cust to syncCC - with data: {JsonConvert.SerializeObject(custIdsToSyncCC)}");

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

        private void RescanNormalAccount(List<int> customers, FLOWTYPERESCANCC flowTypeRescanCC)
        {
            if (customers == null || !customers.Any())
            {
                return;
            }

            var custIdsChunk = customers.Chunk(AppSettings.InternalChunkSize).ToList();

            foreach (var custIds in custIdsChunk)
            {
                var custIdsToBeRescaned = string.Join(",", custIds);

                int funcid = flowTypeRescanCC == FLOWTYPERESCANCC.NEWMEMBER ? 4 : -1;

                _dataCenterService.ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned, funcid, false);
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

            if (_isWriteLogInfo)
                LogService.Info($"PA Rescan - GetAgentCusts - with data: {JsonConvert.SerializeObject(agentIds)}");

            return agentIds;
        }
    }
}
