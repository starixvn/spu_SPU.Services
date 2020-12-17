using Fanex.Logging;
using Fanex.NAP.CTS.DataAccess;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Services.Interface;
using Fanex.NAP.SPU.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.Services
{
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

        public List<int> TransformCustomerFromMainDB(List<CustomerMainDbModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return dataCenterDataAccess.TransformCustomerFromMainDB(customerJson);
        }

        public IEnumerable<TypeCategoryCustomerModel> InsertCategoryForNewCustomer (string customerList)
        {
            return dataCenterDataAccess.InsertCategoryForNewCustomer(customerList);
        }
        

        public int FillUpCustomerFromMainDB(List<CustomerMainDbModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);
            var result = dataCenterDataAccess.FillUpCustomerFromMainDB(customerJson);
            return result;
        }

        public List<CTSCustomer> GetCustomerForUpdateUsername2(int rowSize, bool isUpdateCustSub, int maxCTSCustId)
        {
            return dataCenterDataAccess.GetCustomersForUpdateUsername2(rowSize, isUpdateCustSub, maxCTSCustId);
        }

        public List<CustomerMainDbModel> GetUsername2FromMainDb(string custIds, bool isCustSub)
        {
            return dataCenterDataAccess.GetUsername2FromMainDb(custIds,isCustSub);
        }

        public int UpdateUsername2(List<CustomerMainDbModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return dataCenterDataAccess.UpdateUsername2(customerJson);
        }

        public void InsertAssociation(List<AssociationModel> associationList)
        {
            var associationJson = JsonConvert.SerializeObject(associationList);

            dataCenterDataAccess.InsertAssociation(associationJson);
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

        public void AutoTagClassification(string newClassification)
        {
            dataCenterDataAccess.AutoTagCustomerClassification(newClassification);
        }

        public IEnumerable<CustomerGeneralClassificationModel> InsertMainCustomerClassification(string newClassification)
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

        public void UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<LastestClassificationModel> custIdList)
        {
            dataCenterDataAccess.UpdateCustomerGeneralClassificationtoMainDB(custIdList.ToList());

            //var custClassList = custIdList.Select(c => new CustomerClassModel()
            //{
            //    CustID = c.CustId,
            //    CustomerClass = c.CustomerClass
            //});

            //FeedDataToSoon88(custClassList, retryTime); //TODO separate this logic 
        }

        public void UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<CustomerClassModel> custClassList)
        {
            dataCenterDataAccess.UpdateCustomerGeneralClassificationtoMainDB(custClassList);
            Helper.FeedDataToSoon88Helper.FeedDataToSoon88(custClassList);
        }

        public void InsertProblemAccountClassificationToMainDB(List<ProblemAccountForMainDBModel> custIdList)
        {
            dataCenterDataAccess.InsertProblemAccountCategoryToMainDB(custIdList);

            var custClassList = custIdList.Where(c => c.CategoryIdMapping != -99).Select(c => new CustomerClassModel()
            {
                CustID = c.CustID,
                CustomerClass = c.CategoryIdMapping
            });

            Helper.FeedDataToSoon88Helper.FeedDataToSoon88(custClassList);
        }

        public void InsertProblemCustomerClassificationToMainDB(List<ProblemCustomerForMainDBModel> custIdList)
        {
            dataCenterDataAccess.InsertProblemCustomerCategoryToMainDB(custIdList);

            var custClassList = custIdList.Select(c => new CustomerClassModel()
            {
                CustID = c.CustID,
                CustomerClass = c.CustomerClass
            });

            Helper.FeedDataToSoon88Helper.FeedDataToSoon88(custClassList);
        }

        public IEnumerable<CustomerCategoryClassificationModel> GetCustomerClassByCustId(string custIds)
        {
            return dataCenterDataAccess.GetCustomerClassByCustId(custIds);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerForRunDaily(int batchQuantity, int rowQuantityPerBatch, int parentCateId)
        {
            return dataCenterDataAccess.GetCustomerForDailyScanning(batchQuantity, rowQuantityPerBatch, parentCateId);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerHasChangeForRunRealtime()
        {
            return dataCenterDataAccess.GetCustomerHasChangeForRunRealtime();
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryHasChangeForRunRealtime()
        {
            return dataCenterDataAccess.GetCustomerGeneralCategoryHasChangeForRunRealtime();
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerForRunRealtime(string custIdList)
        {
            return dataCenterDataAccess.GetCustomerForRealtimeScanning(custIdList);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryForRunRealtime(string custIdList)
        {
            return dataCenterDataAccess.GetCustomerGeneralCategoryForRunRealtime(custIdList);
        }

        public void ClassifyCustomerDaily (IEnumerable<CustomerClassificationModel> customers)
        {
            dataCenterDataAccess.ClassifyCustomerDaily(customers);
        }

        public void ClassifyCustomerGeneralCategoryDaily(IEnumerable<CustomerClassificationModel> customers)
        {
            dataCenterDataAccess.ClassifyCustomerGeneralCategoryDaily(customers);
        }

        public void ClassifyCustomerDailyDone(string custSportJson, int parentCateId)
        {
            dataCenterDataAccess.ClassifyCustomerDailyDone(custSportJson, parentCateId);
        }

        public void ClassifyCustomerRealtime(IEnumerable<CustomerClassificationModel> customers)
        {
            dataCenterDataAccess.ClassifyCustomerRealtime(customers);
        }

        public void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustomerClassificationModel> customers)
        {
            dataCenterDataAccess.ClassifyCustomerGeneralCategoryRealtime(customers);
        }

        public void RealtimeUpdateLastScanTime(bool isGeneralCategory)
        {
            dataCenterDataAccess.RealtimeUpdateLastScanTime(isGeneralCategory);
        }

        public IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts(string custIDs, DateTime dateTime)
        {
            return dataCenterDataAccess.GetProblemAccounts(custIDs, dateTime);
        }

        public IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts_CN88(string custIDs, DateTime dateTime)
        {
            return dataCenterDataAccess.GetProblemAccounts_CN88(custIDs, dateTime);
        }

        public IEnumerable<string> InsertProblemAccounts(string jsonData)
        {
            return dataCenterDataAccess.InsertProblemAccount(jsonData);
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

        public IEnumerable<int> GetAllSpecialCustomerClass()
        {
            return dataCenterDataAccess.GetAllSpecialCustomerClass();
        }

        public IEnumerable<RobotUser> GetRobotUser(DateTime fromDate, string creditSiteList)
        {
            return dataCenterDataAccess.GetRobotUsers(fromDate, creditSiteList);
        }

        public IEnumerable<CustomerClassModel> InsertRobotUsers(string custIds)
        {
            return dataCenterDataAccess.InsertRobotUsers(custIds);
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

        public IEnumerable<ProblemAccountForMainDBModel> GetLastestCategory(string ctsCustID, bool isCustId, bool isExcludedDetailedCategory)
        {
            return dataCenterDataAccess.GetLastestCategory(ctsCustID, isCustId, isExcludedDetailedCategory);
        }

        public IEnumerable<LastestClassificationModel> GetLatestClassification(IEnumerable<TypeCategoryCustomerModel> typeCategoryCusts)
        {
            return dataCenterDataAccess.GetLatestClassification(typeCategoryCusts);
        }

        public IEnumerable<AssociationWithCategory> FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses, bool isDelete)
        {
            return dataCenterDataAccess.FeedDataToToSoon88(custClasses, isDelete);
        }

        public DBResult UpdateAppliedCustomerClassification(string jsonData)
        {
            return dataCenterDataAccess.UpdateAppliedCustomerClassification(jsonData);
        }

    }
}
