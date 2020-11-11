namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface IProcessingService
    {
        void ResetOverwrite(bool isOverwrite);

        void ResetPrevious();

        IProcessingService RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        IProcessingService InsertNewCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        IProcessingService SyncCustomerClassToMainDb(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        IProcessingService UpdateProcessedCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);

        IProcessingService UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileModel> custEvidences);
       
    }
}
