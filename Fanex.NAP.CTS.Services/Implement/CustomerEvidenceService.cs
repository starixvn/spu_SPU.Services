namespace Fanex.NAP.CTS.Services
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.DataAccess;
    using Fanex.NAP.CTS.Model;
    using System;
    using System.Collections.Generic;

    public class CustomerEvidenceService : ICustomerEvidenceService
    {
        private CustomerEvidenceDataAccess dataAccess;

        public CustomerEvidenceService()
        {
            dataAccess = new CustomerEvidenceDataAccess();
        }

        public IEnumerable<CustomerEvidenceFromFileModel> GetCustomerEvidenceFromFile()
        {
            return dataAccess.GetCustomerEvidenceFromFile();
        }

        public DBResult RemoveCustomerEvidenceByCTSCustIds(string jsonData, long sessionId, bool isRollBack, bool isClearLog)
        {
            try
            {
                return dataAccess.RemoveCustomerEvidenceByCTSCustIds(jsonData, sessionId, isRollBack, isClearLog);
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidenceService RemoveCustomerEvidenceByCTSCustIds exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult InsertCustomerEvidenceMuitiple(string jsonData, int userId, bool isOverwrite)
        {
            try
            {
                return dataAccess.InsertCustomerEvidenceMuitiple(jsonData, userId, isOverwrite);
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidenceService InsertCustomerEvidenceMuitiple exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult SyncCustomerEvidenceToCategory(string jsonData)
        {
            try
            {
                return dataAccess.SyncCustomerEvidenceToCategory(jsonData);
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidenceService SyncCustomerEvidenceToCategory exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult SyncCustomerClassToMainDb(string ctsCustIds)
        {
            try
            {
                var custClassToXml = "";

                return dataAccess.SyncCustomerClassToMainDb(custClassToXml);
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidenceService SyncCustomerClassToMainDb exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult UpdateProcessedCustEvidence(string jsonData, int userId)
        {
            try
            {
                return dataAccess.UpdateProcessedCustEvidence(jsonData, userId);
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidenceService UpdateCustEvidenceToProcess exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public IEnumerable<int> GetAssociatedAccountByCTSCustIds(string ctsCustIds)
        {
            try
            {
                return dataAccess.GetAssociatedAccountByCTSCustIds(ctsCustIds);
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidenceService GetAssociatedAccountByCTSCustIds exception: {ex.Message}", ex);

                return new List<int>();
            }
        }

        public DBResult UpdateBetLimitOfFlaggedCustomer(string custIds)
        {
            try
            {
                return dataAccess.UpdateBetLimitOfFlaggedCustomer(custIds);
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidenceService UpdateBetLimitOfFlaggedCustomer exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }
    }
}
