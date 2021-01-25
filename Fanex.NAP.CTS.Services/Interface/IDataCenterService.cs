namespace Fanex.NAP.CTS.Services.Interface
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.SPU.Model;
    using System;
    using System.Collections.Generic;
    public interface IDataCenterService
    {
        void TransformDCSAccount(List<RawAccountModel> accountList);

        IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(List<CustomerMainDbModel> customerList);

        void InsertAssociation(List<AssociationModel> associationList);

        void TransformAssociationByDevice(List<RawAssociationModel> rawAssociation);

        void TransformDCSTransactionIPToCTSAssociationByIP(string transactionByIpJson);

        List<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId);

        List<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs);

        int UpdateUsername2(List<GetCustIdModel> customerList, int size);

        List<CustomerMainDbModel> GetUsername2FromMainDb(string custIds, bool isCustSub);

        IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize);

        void UpdateLastLoginTime(int batchSize);

        List<CustomerClassificationModelMoreInfo> GetNewClassification();

        string GetNewMemberAssociationProblemAccount(string custIds);

        void UpdateAppliedStatusClassification(string custIdList, bool isGeneralCategory);

        void AutoTagClassification(string newClassification);

        InsertCategoryResultModel InsertMainCustomerClassification(string newClassification, bool isRescan, string associationPA);

        DBResult UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<LatestClassificationModel> custIdList);

        void InsertProblemAccountClassificationToMainDB(List<ProblemAccountForMainDBModel> custIdList);

        IEnumerable<TransactionModel> DCSMoveTransToTransaction07GetPackage(int batchSize, int NoOfBatch);

        void DCSMoveTransToTransaction07(string transIdList);

        IEnumerable<CustomerClassificationModel> GetCustomerForRunDaily(int batchQuantity, int rowQuantityPerBatch);

        IEnumerable<CustomerClassificationModel> GetCustomerForRunRealtime(string custIdList);

        void ClassifyCustomerDaily(IEnumerable<CustomerClassificationModel> customers);

        void ClassifyCustomerDailyDone(string custSportJson);

        void ClassifyCustomerGeneralCategoryDaily(string customers, bool isCheckLast14Days);

        void ClassifyCustomerRealtime(IEnumerable<CustomerClassificationModel> customers);

        void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustomerClassificationModel> customers);

        IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryForRunRealtime(string custIdList, string categoryIds);        

        IEnumerable<int> GetAllSpecialCustomerClass();

        IEnumerable<CustomerCategoryClassificationModel> GetCustomerClassByCustId(string custIdList);

        IEnumerable<ProblemAccountForMainDBModel> GetLastestCategory(string ctsCustID, bool isCustId, bool isExcludedDetailedCategory);

        IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds);

        IEnumerable<AssociationWithCategory> FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses, bool isDelete);

        void UpdateAppliedCustomerClassification(string jsonData);

        IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts(string custIDs, DateTime? dateTime);

        IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime);

        InsertCategoryResultModel InsertProblemAccounts(IEnumerable<InsertProblemAccountsModel> problemAccounts, bool isRescan);

        IEnumerable<CustEvidenceModel> GetExistedEvidences(string custIDs);

        IEnumerable<SystemParemeterModel> GetPreviousTimingExecution(int paramId);

        void UpdateTimingExecution(string newDataTime, int paramId);

        void ReTransformAccount(string jsonData);

        IEnumerable<CustomerMainDbModel> GetCustomerFromMainDB(string custIds);

        IEnumerable<CustomerMainDbModel> GetCustomerFromCTS(string custIds);
        
        void UpdateCustomerInfo(string customers, DateTime lastUpdateTime, Int64 lastUpdateCustID);
    }
}
