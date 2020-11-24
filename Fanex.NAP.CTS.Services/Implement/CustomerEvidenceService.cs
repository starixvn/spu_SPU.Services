namespace Fanex.NAP.CTS.Services
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.DataAccess;
    using Fanex.NAP.CTS.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                var customerCategoryLastedList = dataAccess.GetCustomerCategoryLasted(ctsCustIds, false, true);

                if (customerCategoryLastedList != null && customerCategoryLastedList.Any())
                {
                    var ccToMain = customerCategoryLastedList.Select(x => new SyncCustomerClassToMainModel
                    {
                        CustID = x.CustId,
                        CategoryIdMapping = GetMappingCTSCategoryToMainCC(x.CategoryId)
                    });

                    return dataAccess.SyncCustomerClassToMainDb(ccToMain);
                }

                return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            }
            catch (Exception ex)
            {
                LogService.Error($"CustomerEvidenceService SyncCustomerClassToMainDb exception: {ex.Message}", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public DBResult UpdateProcessedCustEvidence(string idList, int userId)
        {
            try
            {
                return dataAccess.UpdateProcessedCustEvidence(idList, userId);
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

        private int GetMappingCTSCategoryToMainCC(int categoryId)
        {
            switch (categoryId)
            {
                case 52:
                case 72:
                case 152:
                    return 8;
                case 54:
                case 58:
                case 74:
                case 78:
                case 154:
                case 158:
                    return 9;
                case 53:
                case 55:
                case 59:
                case 73:
                case 75:
                case 79:
                case 153:
                case 155:
                case 159:
                    return 10;
                case 56:
                case 60:
                case 76:
                case 80:
                case 156:
                    return 11;
                case 57:
                case 157:
                    return 13;
                case -1:
                    return -1;
                default:
                    return -99;
            }
        }
    }
}
