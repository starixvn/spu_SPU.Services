using Fanex.NAP.CTS.Model;
using Fanex.NAP.SPU.Model;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Services.Interface
{
    public interface IDataCenterService
    {
        void TransformDCSAccount(List<RawAccountModel> accountList);

        List<int> TransformCustomerFromMainDB(List<CustomerMainDbModel> customerList);

        int FillUpCustomerFromMainDB(List<CustomerMainDbModel> customerList);

        void InsertAssociation(List<AssociationModel> associationList);

        void TransformAssociationByDevice(List<RawAssociationModel> rawAssociation);

        void TransformDCSTransactionIPToCTSAssociationByIP(string transactionByIpJson);

        List<CTSCustomer> GetCustomerForUpdateUsername2(int rowSize, bool isUpdateCustSub, int maxCTSCustId);

        int UpdateUsername2(List<CustomerMainDbModel> customerList);

        List<CustomerMainDbModel> GetUsername2FromMainDb(string custIds, bool isCustSub);

        IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize);

        void UpdateLastLoginTime(int batchSize);

        List<CustomerClassificationModelMoreInfo> GetNewClassification();

        void UpdateAppliedStatusClassification(string custIdList, bool isGeneralCategory);

        void AutoTagClassification(string newClassification);

        IEnumerable<CustomerGeneralClassificationModel> InsertMainCustomerClassification(string newClassification);

        void UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<CustomerGeneralClassificationModel> custIdList);

        void InsertProblemAccountClassificationToMainDB(List<ProblemAccountForMainDBModel> custIdList);

        IEnumerable<TransactionModel> DCSMoveTransToTransaction07GetPackage(int batchSize, int NoOfBatch);

        void DCSMoveTransToTransaction07(string transIdList);

        IEnumerable<CustomerClassificationModel> GetCustomerForRunDaily(int batchQuantity, int rowQuantityPerBatch, int parentCateId);

        IEnumerable<CustomerClassificationModel> GetCustomerForRunRealtime(string custIdList);
        

        void ClassifyCustomerDaily(IEnumerable<CustomerClassificationModel> customers);

        void ClassifyCustomerDailyDone(string custSportJson, int parentCateId);

        void ClassifyCustomerRealtime(IEnumerable<CustomerClassificationModel> customers);

        void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustomerClassificationModel> customers);

        IEnumerable<CustomerClassificationModel> GetCustomerGeneralCategoryForRunRealtime(string custIdList);

        List<int> InsertCategoryForNewCustomer(string customerList);

        IEnumerable<int> GetAllSpecialCustomerClass();

        IEnumerable<CustomerCategoryClassificationModel> GetCustomerClassByCustId(string custIdList);

        IEnumerable<ProblemAccountForMainDBModel> GetLastestCategory(string ctsCustID, bool isCustId, bool isExcludedDetailedCategory);

        IEnumerable<ProblemCustomerForMainDBModel> GetLatestClassification(string ctsCustID, bool isCustId);

        IEnumerable<AssociationWithCategory> FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses, bool isDelete);
        
    }
}
