using Fanex.NAP.CTS.Model;
using Fanex.NAP.SPU.Model;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Services.Interface
{
    public interface IDataCenterService
    {
        void TransformDCSAccount(List<RawAccountModel> accountList);

        List<int> TransformCustomerFromMainDB(List<CustomerMainDbModel> customerList);

        List<int> FillUpCustomerFromMainDB(List<CustomerMainDbModel> customerList);

        void InsertAssociation(List<AssociationModel> associationList);

        void TransformAssociationByDevice(List<RawAssociationModel> rawAssociation);

        void TransformDCSTransactionIPToCTSAssociationByIP(string transactionByIpJson);

        List<CTSCustomer> GetCustomerForUpdateUsername2(int rowSize, bool isUpdateCustSub, int maxCTSCustId);

        int UpdateUsername2(List<CustomerMainDbModel> customerList);

        List<CustomerMainDbModel> GetUsername2FromMainDb(string custIds, bool isCustSub);

        void TransformAssociationAffectedEvidence(int batchSize);

        void UpdateLastLoginTime(int batchSize);

        List<CustomerClassificationModelMoreInfo> GetNewClassification();

        void UpdateNewClassification(string custIdList);

        void AutoTagClassification(string newClassification);

        IEnumerable<TransactionModel> DCSMoveTransToTransaction07GetPackage(int batchSize, int NoOfBatch);

        void DCSMoveTransToTransaction07(string transIdList);

        IEnumerable<CustomerClassificationModel> GetCustomerForRunDaily(int batchQuantity, int rowQuantityPerBatch);

        IEnumerable<CustomerClassificationModel> GetCustomerForRunRealtime(string custIdList);
        

        void ClassifyCustomerDaily(IEnumerable<CustomerClassificationModel> customers);

        void ClassifyCustomerDailyDone(string custSportJson);

        void ClassifyCustomerRealtime(IEnumerable<CustomerClassificationModel> customers);

        List<int> InsertCategoryForNewCustomer(string customerList);

        void RealtimeUpdateLastScanTime();
    }
}
