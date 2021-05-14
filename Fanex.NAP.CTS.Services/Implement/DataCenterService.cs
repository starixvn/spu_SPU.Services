namespace Fanex.NAP.CTS.Services
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.DataAccess;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.Core.Constants;

    public class DataCenterService : IDataCenterService
    {
        private readonly DataCenterDataAccess dataCenterDataAccess;

        public DataCenterService()
        {
            dataCenterDataAccess = new DataCenterDataAccess();
        }

        public void TransformAssociationByDevice(List<RawAssociationModel> rawAssociation)
        {
            var associationJson = JsonConvert.SerializeObject(rawAssociation);

            dataCenterDataAccess.TransformAssociationByDevice(associationJson);
        }

        public void TransformDCSAccount(List<RawAccountModel> accountList)
        {
            var accountJson = JsonConvert.SerializeObject(accountList);

            dataCenterDataAccess.TransformDCSAccount(accountJson);
        }

        public IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(List<CustomerMainDBModel> customerList)
        {
            var custInfos = new List<InsertCustomerModel>();

            try
            {
                var customerJson = JsonConvert.SerializeObject(customerList);

                custInfos = dataCenterDataAccess.TransformCustomerFromMainDB(customerJson)?.ToList();
            }
            catch (Exception ex)
            {
                LogService.Error($"Service - InsertNewCustomer - TransformCustomerFromMainDB exception: {ex.Message} - Data: {JsonConvert.SerializeObject(customerList)}", ex);
            }

            return custInfos;
        }

        public List<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId)
        {
            return dataCenterDataAccess.GetCustWithNullUserName2(rowSize, preCTSCustId);
        }

        public List<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs)
        {
            return dataCenterDataAccess.GetCustWithHavingUserName2(custs);
        }

        public int UpdateUsername2(List<GetCustIdModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return dataCenterDataAccess.UpdateUsername2(customerJson);
        }

        public void TransformDCSTransactionIPToCTSAssociationByIP(string transactionByIpJson)
        {           
            dataCenterDataAccess.TransformDCSTransactionIPToCTSAssociationByIP(transactionByIpJson);
        }

        public IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize)
        {
            return dataCenterDataAccess.TransformAssociationAffectedEvidence(batchSize);
        }

        public void UpdateBetLimitByEvidenceRemoving(IEnumerable<int> custIds)
        {
            dataCenterDataAccess.UpdateBetLimitByEvidenceRemoving(custIds);
        }

        public void UpdateLastLoginTime(int batchSize)
        {
            dataCenterDataAccess.TransformUpdatingLastLoginTime(batchSize);
        }

        public List<CustomerClassificationModelMoreInfo> GetNewClassification()
        {
            return dataCenterDataAccess.GetNewClassification();
        }

        public InsertCategoryResultModel InsertMainCustomerClassification(string newClassification)
        {
           return dataCenterDataAccess.InsertMainCustomerClassification(newClassification);
        }

        public void DCSMoveTransToTransaction07(string transIdList)
        {
            dataCenterDataAccess.DCSMoveTransToTransaction07(transIdList);
        }

        public IEnumerable<TransactionModel> DCSMoveTransToTransaction07GetPackage(int batchSize, int NoOfBatch)
        {
            return dataCenterDataAccess.DCSMoveTransToTransaction07GetPackage(batchSize, NoOfBatch);
        }

        public void UpdateAppliedStatusClassification(string custIdList, bool isGeneralCategory)
        {
            dataCenterDataAccess.UpdateAppliedStatusClassification(custIdList, isGeneralCategory);
        }

        public DBResult UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<LatestClassificationModel> custIdList)
        {
            return dataCenterDataAccess.UpdateCustomerGeneralClassificationtoMainDB(custIdList.ToList());
        }

        public void UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<CustomerClassModel> custClassList)
        {
            dataCenterDataAccess.UpdateCustomerGeneralClassificationtoMainDB(custClassList);
            Helper.FeedDataToSoon88Helper.FeedDataToSoon88(custClassList);
        }

        public IEnumerable<CustomerCategoryClassificationModel> GetCustomerClassByCustId(string custIds)
        {
            return dataCenterDataAccess.GetCustomerClassByCustId(custIds);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerForRunDaily(int batchQuantity, int rowQuantityPerBatch)
        {
            return dataCenterDataAccess.GetCustomerForDailyScanning(batchQuantity, rowQuantityPerBatch);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryHasChangeForRunRealtime()
        {
            return dataCenterDataAccess.GetCustomerGeneralCategoryHasChangeForRunRealtime();
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryForRunRealtime(string custIdList, string categoryIds)
        {
            return dataCenterDataAccess.GetCustomerGeneralCategoryForRunRealtime(custIdList, categoryIds);
        }

        public void ClassifyCustomerGeneralCategoryDaily(string customers, bool isCheckLast14Days)
        {
            dataCenterDataAccess.ClassifyCustomerGeneralCategoryDaily(customers, isCheckLast14Days);
        }

        public void ClassifyCustomerDailyDone(string custSportJson)
        {
            dataCenterDataAccess.ClassifyCustomerDailyDone(custSportJson);
        }

        public void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustomerClassificationModel> customers)
        {
            dataCenterDataAccess.ClassifyCustomerGeneralCategoryRealtime(customers);
        }

        public void RealtimeUpdateLastScanTime(bool isGeneralCategory)
        {
            dataCenterDataAccess.RealtimeUpdateLastScanTime(isGeneralCategory);
        }

        public IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts(string custIDs, DateTime? dateTime)
        {
            return dataCenterDataAccess.GetProblemAccounts(custIDs, dateTime);
        }

        public IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime)
        {
            return dataCenterDataAccess.GetProblemAccounts_CN88(custIDs, dateTime);
        }

        public InsertCategoryResultModel InsertProblemAccounts(IEnumerable<MultipleProblemAccountsModel> problemAccounts, bool isRescan)
        {
            var result = new InsertCategoryResultModel() {
                ErrorMessage = string.Empty, 
                CustIds = new List<int>() 
            };

            try
            {
                if (problemAccounts == null || !problemAccounts.Any())
                {
                    return result;
                }
              
                result = dataCenterDataAccess.InsertProblemAccount(JsonConvert.SerializeObject(problemAccounts), isRescan);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    LogService.Error($"Service - Rescan PA - Insert Problem Acocunts exception: {result.ErrorMessage} - Data: {JsonConvert.SerializeObject(problemAccounts)}");
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"Service - Rescan PA - Insert Problem Acocunts exception: {ex.Message} - Data: {JsonConvert.SerializeObject(problemAccounts)}", ex);
            }

            return result;
        }

        public IEnumerable<SystemParemeterModel> GetPreviousTimingExecution(int paramId)
        {
            return dataCenterDataAccess.GetPreviousTimingExecution(paramId);
        }

        public void UpdateTimingExecution (string newDataTime, int paramId)
        {
            dataCenterDataAccess.UpdateTimingExecution(newDataTime, paramId);
        }

        public void InsertUserPermission (string permissionUserIdsJson)
        {
            dataCenterDataAccess.InsertUserPermission(permissionUserIdsJson);
        }

        public IEnumerable<RobotUser> GetRobotUser(DateTime fromDate, string creditSiteList)
        {
            return dataCenterDataAccess.GetRobotUsers(fromDate, creditSiteList);
        }

        public IEnumerable<ProbationModel> GetProbationCustForScanning(int numberOfRecords)
        {
            return dataCenterDataAccess.GetProbationCustForScanning(numberOfRecords);
        }

        public void UpdateLastScanDateForProbationCust(IEnumerable<ProbationModel> custList)
        {
            dataCenterDataAccess.UpdateLastScanDateForProbationCust(custList);
        }

        public IEnumerable<ProbationModel> ClassifyCategoryForProbationCust(IEnumerable<ProbationModel> custList)
        {
            return dataCenterDataAccess.ClassifyCategoryForProbationCust(custList);
        }

        public void InsertProbationNotification(IEnumerable<ProbationModel> scanList, IEnumerable<ProbationModel> resultList)
        {
            dataCenterDataAccess.InsertProbationNotification(scanList, resultList);
        }

        public IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds)
        {
            return dataCenterDataAccess.GetLatestClassification(custIds);
        }

        public IEnumerable<DangerLevelModel> GetDangerLevel(string custIds, DANGERLEVEL typeID)
        {
            return dataCenterDataAccess.GetDangerLevel(custIds, typeID);
        }

        public IEnumerable<AssociationWithCategory> FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses, bool isDelete)
        {
            return dataCenterDataAccess.FeedDataToToSoon88(custClasses, isDelete);
        }

        public void UpdateAppliedCustomerClassification(string jsonData)
        {
            var resultAppliedCC =  dataCenterDataAccess.UpdateAppliedCustomerClassification(jsonData);

            if (resultAppliedCC.ErrorCode != 0)
            {
                LogService.Error($"Service - Update Applied Customer Classification fails - Exception {resultAppliedCC.ErrorMsg} with data: {jsonData}");
            }
        }

        public void UpdateAppliedDangerLevel(string custs, DANGERLEVEL typeID)
        {
            dataCenterDataAccess.UpdateAppliedDangerLevel(custs, typeID);
        }

        public IEnumerable<CustEvidenceModel> GetExistedEvidences(string custIDs)
        {
            return dataCenterDataAccess.GetExistedEvidences(custIDs);
        }

        public IEnumerable<CustomerAssociationProblemAccountModel> GetCustomerAssociationProblemAccount(string custIds)
        {            
            return dataCenterDataAccess.GetCustomerAssociationProblemAccount(custIds);
        }
        
        public void ReTransformAccount(string jsonData, long? lastCustID)
        {
            dataCenterDataAccess.ReTranformAccount(jsonData, lastCustID);
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromMainDB(string custIds, UPDATECUSTOMER sourceType)
        {
            return dataCenterDataAccess.GetCustomerFromMainDB(custIds, sourceType);
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromCTS(string custIds, UPDATECUSTOMER sourceType)
        {
            return dataCenterDataAccess.GetCustomerFromCTS(custIds, sourceType);
        }

        public void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID, UPDATECUSTOMER sourceType)
        {
            if (sourceType == UPDATECUSTOMER.FROM_CUSTOMER)
            {
                dataCenterDataAccess.UpdateCustomerComplete(customers, lastUpdateTime, lastUpdateCustID);
            }
            else
            {
                dataCenterDataAccess.UpdateCustInfoComplete(customers, lastUpdateTime, lastUpdateCustID);
            }
        }

        public IEnumerable<RoleCustomerModel> GetRoleCustomer(string custIDs)
        {
            return dataCenterDataAccess.GetRoleCustomer(custIDs);
        }

        public IEnumerable<int> GetCustByRobotSiteRule(string custIDs)
        {
            return dataCenterDataAccess.GetCustByRobotSiteRule(custIDs);
        }

        public string GetExistedRobotUser(string custIDs)
        {
            return dataCenterDataAccess.GetExistedRobotUser(custIDs);
        }
    }
}
