namespace Fanex.NAP.CTS.Services
{
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    public interface ICustomerEvidenceService
    {
        IEnumerable<CustomerEvidenceFromFileModel> GetCustomerEvidenceFromFile();

        DBResult RemoveCustomerEvidenceByCTSCustIds(string jsonData, long sessionId, bool isRollBack, bool isClearLog);

        DBResult InsertCustomerEvidenceMuitiple(string jsonData, int userId, bool isOverwrite);

        DBResult SyncCustomerClassToMainDb(string ctsCustIds);

        DBResult UpdateProcessCustEvidence(string custEvidenceIds, int userId);

        DBResult UpdateBetLimitOfFlaggedCustomer(string custIds);
    }
}
