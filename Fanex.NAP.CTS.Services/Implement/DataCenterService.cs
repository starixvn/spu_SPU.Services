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

        public List<int> InsertCategoryForNewCustomer(string customerList)
        {
            return dataCenterDataAccess.InsertCategoryForNewCustomer(customerList);
        }
        

        public List<int> FillUpCustomerFromMainDB(List<CustomerMainDbModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return dataCenterDataAccess.FillUpCustomerFromMainDB(customerJson);
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

        public void UpdateCustomerGeneralClassificationtoMainDB(List<CustomerGeneralClassificationModel> custIdList)
        {
            dataCenterDataAccess.UpdateCustomerGeneralClassificationtoMainDB(custIdList);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerForRunDaily(int batchQuantity, int rowQuantityPerBatch, int parentCateId)
        {
            return dataCenterDataAccess.GetCustomerForDailyScanning(batchQuantity, rowQuantityPerBatch, parentCateId);
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerHasChangeForRunRealtime()
        {
            return dataCenterDataAccess.GetCustomerHasChangeForRunRealtime();
        }

        public IEnumerable<CustomerClassificationModel> GetCustomerForRunRealtime(string custIdList)
        {
            return dataCenterDataAccess.GetCustomerForRealtimeScanning(custIdList);
        }

        public void ClassifyCustomerDaily (IEnumerable<CustomerClassificationModel> customers)
        {
            dataCenterDataAccess.ClassifyCustomerDaily(customers);
        }

        public void ClassifyCustomerDailyDone(string custSportJson, int parentCateId)
        {
            dataCenterDataAccess.ClassifyCustomerDailyDone(custSportJson, parentCateId);
        }

        public void ClassifyCustomerRealtime(IEnumerable<CustomerClassificationModel> customers)
        {
            dataCenterDataAccess.ClassifyCustomerRealtime(customers);
        }

        public void RealtimeUpdateLastScanTime()
        {
            dataCenterDataAccess.RealtimeUpdateLastScanTime();
        }

        public IEnumerable<ProblemAccountsModel> GetProblemAccounts(string custIDs, DateTime dateTime)
        {
            return dataCenterDataAccess.GetProblemAccounts(custIDs, dateTime);
        }

        public void InsertProblemAccounts(string jsonData)
        {
            dataCenterDataAccess.InsertProblemAccount(jsonData);
        }

        public IEnumerable<SystemParemeterModel> GetPreviousTimingExecution()
        {
            return dataCenterDataAccess.GetPreviousTimingExecution();
        }

        public void UpdateTimingExecution (string newDataTime)
        {
            dataCenterDataAccess.UpdateTimingExecution(newDataTime);
        }

        public void InsertUserPermission (string userIds, string permissionFuncName)
        {
            dataCenterDataAccess.InsertProblemAccount(userIds, permissionFuncName);
        }
    }
}
