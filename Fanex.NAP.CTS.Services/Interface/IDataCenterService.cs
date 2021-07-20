namespace Fanex.NAP.CTS.Services.Interface
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.SPU.Model;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.Core.Constants;

    public interface IDataCenterService
    {
        void TransformDCSAccount(List<RawAccountModel> accountList);

        IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(List<CustomerMainDBModel> customerList);

        void TransformAssociationByDevice(List<RawAssociationModel> rawAssociation);

        void TransformDCSTransactionIPToCTSAssociationByIP(string transactionByIpJson);

        List<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId);

        List<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs);

        int UpdateUsername2(List<GetCustIdModel> customerList);

        IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize);

        void UpdateLastLoginTime(int batchSize);

        List<CustomerClassificationModelMoreInfo> GetNewClassification();

        IEnumerable<CustomerAssociationProblemAccountModel> GetCustomerAssociationProblemAccount(string custIds);

        void UpdateAppliedStatusClassification(string custIdList, bool isGeneralCategory);

        InsertCategoryResultModel InsertMainCustomerClassification(string newClassification);

        DBResult UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<LatestClassificationModel> custIdList);

        IEnumerable<TransactionModel> DCSMoveTransToTransaction07GetPackage(int batchSize, int NoOfBatch);

        void DCSMoveTransToTransaction07(string transIdList);

        IEnumerable<CustomerClassificationModel> GetCustomerForRunDaily(int batchQuantity, int rowQuantityPerBatch);

        void ClassifyCustomerDailyDone(string custSportJson);

        void ClassifyCustomerGeneralCategoryDaily(string customers, int funcId, bool isCheckLast14Days);

        void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustomerClassificationModel> customers);

        IEnumerable<CustomerClassificationModel> GetCustomerClassificationForRunRealtime(string custSportJson);        

        IEnumerable<CustomerCategoryClassificationModel> GetCustomerClassByCustId(string custIdList);

        IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds);

        IEnumerable<DangerLevelModel> GetDangerLevel(string custIds, DANGERLEVEL typeID);

        IEnumerable<AssociationWithCategory> FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses, bool isDelete);

        void UpdateAppliedCustomerClassification(string jsonData);

        void UpdateAppliedDangerLevel(string custs, DANGERLEVEL typeID);

        IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts(string custIDs, DateTime? dateTime);

        IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime);

        InsertCategoryResultModel InsertProblemAccounts(IEnumerable<MultipleProblemAccountsModel> problemAccounts, bool isRescan);

        IEnumerable<CustEvidenceModel> GetExistedEvidences(string custIDs);

        IEnumerable<SystemParemeterModel> GetPreviousTimingExecution(int paramId);

        void UpdateTimingExecution(string newDataTime, int paramId);

        void ReTransformAccount(string jsonData, long? lastCustID);

        IEnumerable<CustomerMainDBModel> GetCustomerFromMainDB(string custIds, UPDATECUSTOMER sourceType);

        IEnumerable<CustomerMainDBModel> GetCustomerFromCTS(string custIds, UPDATECUSTOMER sourceType);
        
        void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID, UPDATECUSTOMER sourceType);

        IEnumerable<RoleCustomerModel> GetRoleCustomer(string custIDs);

        IEnumerable<int> GetCustByRobotSiteRule(string custIDs);

        string GetExistedRobotUser(string custIDs);
    }
}
