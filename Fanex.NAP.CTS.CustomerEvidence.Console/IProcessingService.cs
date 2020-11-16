namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface IProcessingService
    {
        bool RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        bool InsertNewCustomerEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, bool isOverwrite, bool isPrevSuccess);

        bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, int actionType, bool isPrevSuccess);

        IProcessingService SyncCustomerClassToMainDb(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        IProcessingService UpdateProcessedCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        IProcessingService UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        List<CustomerEvidenceFailedToProcessModel> GetLogError();

    }
}
