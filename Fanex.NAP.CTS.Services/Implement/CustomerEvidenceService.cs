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
                LogService.Info($"DataCenterService RemoveCustomerEvidenceByCTSCustIds exception: {ex.Message}", ex);

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
                LogService.Info($"DataCenterService InsertCustomerEvidenceMuitiple exception: {ex.Message}", ex);

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
                LogService.Info($"DataCenterService SyncCustomerClassToMainDb exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult UpdateProcessCustEvidence(string custEvidenceIds, int userId)
        {
            try
            {
                return dataAccess.UpdateProcessCustEvidence(custEvidenceIds, userId);
            }
            catch (Exception ex)
            {
                LogService.Info($"DataCenterService UpdateCustEvidenceToProcess exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
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
                LogService.Info($"DataCenterService UpdateBetLimitOfFlaggedCustomer exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }
    }
}
