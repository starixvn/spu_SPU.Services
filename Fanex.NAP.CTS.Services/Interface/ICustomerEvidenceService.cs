namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.RemoveEvidenceMultipleAccounts;
    using System.Collections.Generic;

    public interface ICustomerEvidenceService
    {
        IEnumerable<CustomerEvidenceFromFileModel> GetCustomerEvidenceFromFile();

        ResultRemoveCustomerEvidence RemoveCustomerEvidenceByCTSCustIds(string jsonData, long sessionId, bool isRollBack, bool isClearLog);

        DBResult InsertCustomerEvidenceMuitiple(string jsonData, int userId, bool isOverwrite);

        DBResult SyncCustomerEvidenceToCategory(string jsonData, bool isRescan);

        InsertCategoryResultModel SyncCustomerEvidenceToCategory(List<CustEvidenceModel> existedEvidences, bool isRescan);

        DBResult UpdateProcessedCustEvidence(string jsonData, bool isProcessed, int userId);

        IEnumerable<int> GetAssociatedAccountByCTSCustIds(string ctsCustIds);

        DBResult UpdateBetLimitOfFlaggedCustomer(string custIds);

        void FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses);

        IEnumerable<CustomerEvidenceQueueModel> GetCustomerEvidenceQueue();

        DBResult ModifyCustomerEvidence(string customerEvidence, int actionId, out string custIdResultList);

        DBResult RollbackCustomerEvidence(long sessionId);

        DBResult UpdateCustomerEvidence(string processedIdList);
    }
}
