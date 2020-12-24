namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface ICustomerEvidenceService
    {
        IEnumerable<CustomerEvidenceFromFileModel> GetCustomerEvidenceFromFile();

        ResultRemoveCustomerEvidence RemoveCustomerEvidenceByCTSCustIds(string jsonData, long sessionId, bool isRollBack, bool isClearLog);

        DBResult InsertCustomerEvidenceMuitiple(string jsonData, int userId, bool isOverwrite);

        DBResult SyncCustomerEvidenceToCategory(string jsonData);

        InsertCategoryResultModel InsertCustEvidenceToCategory(List<CustEvidenceModel> existedEvidences);

        DBResult SyncCustomerClassToMainDb(string ctsCustIds, out List<SyncCustomerClassToMainModel> custClasses);

        DBResult UpdateProcessedCustEvidence(string jsonData, bool isProcessed, int userId);

        IEnumerable<int> GetAssociatedAccountByCTSCustIds(string ctsCustIds);

        DBResult UpdateBetLimitOfFlaggedCustomer(string custIds);

        void FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses);
    }
}
