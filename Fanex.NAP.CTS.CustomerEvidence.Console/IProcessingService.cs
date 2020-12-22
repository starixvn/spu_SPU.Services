namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface IProcessingService
    {
        bool RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, out IEnumerable<CustomerEvidenceRemovedModel> custEviRemovedList);

        bool InsertNewCustomerEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, bool isOverwrite, bool isPrevSuccess);

        bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, int actionType, IEnumerable<CustomerEvidenceRemovedModel> custEviRemovedList, bool isPrevSuccess);

        bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, int actionType, bool isPrevSuccess);

        IProcessingService PushCustomerClassifyToExternal(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        IProcessingService FeedDataToSoon88(IEnumerable<CustomerClassModel> custClasses);

        IProcessingService UpdateProcessedCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, bool isProcessed);

        IProcessingService UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        List<CustomerEvidenceFailedToProcessModel> GetLogError();

    }
}
