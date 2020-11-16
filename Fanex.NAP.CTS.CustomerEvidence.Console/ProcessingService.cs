namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Newtonsoft.Json;

    public class ProcessingService : IProcessingService
    {
        private ICustomerEvidenceService _custEvidenceService;
        private List<CustomerEvidenceFailedToProcessModel> _failedToProcessList;

        public ProcessingService()
        {
            _custEvidenceService = new CustomerEvidenceService();
            _failedToProcessList = new List<CustomerEvidenceFailedToProcessModel>();
        }

        public bool RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            var ctsCustIdList = custEvidences.Select(x => new { x.CTSCustID }).Distinct();

            var jsonData = JsonConvert.SerializeObject(ctsCustIdList);

            var sessionId = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            var dbResult = _custEvidenceService.RemoveCustomerEvidenceByCTSCustIds(jsonData, sessionId, false, false);            

            if (dbResult.ErrorCode != 0)
            {
                WriteLogError("RemoveOldEvidence", dbResult.ErrorMsg, custEvidences);

                return false;
            }

            return true;
        }

        public bool InsertNewCustomerEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, bool isOverwrite, bool isPrevSuccess)
        {
            if (isPrevSuccess)
            {
                var starixITId = 10278938;
                var count = 0;
                var isSuccess = true;
                var isDeadlock = false;
                var errorMsg = string.Empty;

                var objects = custEvidences.Select(x => new
                {
                    x.CTSCustID,
                    x.EvidenceID,
                    x.Remark
                });

                var jsonData = JsonConvert.SerializeObject(objects);
                
                do
                {
                    var dbResult = _custEvidenceService.InsertCustomerEvidenceMuitiple(jsonData, starixITId, isOverwrite);

                    isSuccess = dbResult.ErrorCode == 0;
                    errorMsg = dbResult.ErrorMsg;
                    isDeadlock = errorMsg != null && errorMsg.ToLower().Contains("deadlock");

                    if (isDeadlock)
                        Thread.Sleep(2000);
                    count++;
                } while (count < 3 && isDeadlock);

                if (!isSuccess)
                {
                    WriteLogError("InsertNewCustEvidence", errorMsg, custEvidences);

                    return false;
                }

                return true;
            }

            return isPrevSuccess;
        }

        /// <summary>
        /// Sync To CTS Category
        /// </summary>
        /// <param name="custEvidences"></param>
        /// <param name="actionType">0: Insert, 2: Remove</param>
        /// <returns></returns>
        public bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, int actionType, bool isPrevSuccess)
        {
            if (isPrevSuccess)
            {
                var objects = custEvidences.Select(x => new
                {
                    x.CustID,
                    x.CTSCustID,
                    x.SubscriberID,
                    x.EvidenceID,
                    ActionType = actionType
                });

                var jsonData = JsonConvert.SerializeObject(objects);

                var dbResult = _custEvidenceService.SyncCustomerEvidenceToCategory(jsonData);

                if(dbResult.ErrorCode != 0)
                {
                    WriteLogError("InsertNewCustEvidence", dbResult.ErrorMsg, custEvidences);

                    return false;
                }
                return true;
            }

            return isPrevSuccess;
        }

        public IProcessingService SyncCustomerClassToMainDb(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            var ctsCustIdList = custEvidences.Select(x => new { x.CTSCustID }).Distinct();

            var ctsCustIds = string.Join(",", ctsCustIdList.Distinct().ToList());

            var dbResult = _custEvidenceService.SyncCustomerClassToMainDb(ctsCustIds);

            if (dbResult.ErrorCode != 0)
            {
                WriteLogError("SyncCustomerClassToMainDb", dbResult.ErrorMsg, custEvidences);
            }

            return this;
        }

        public IProcessingService UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            var ctsCustIds = string.Join(",", custEvidences.Select(x => x.CTSCustID).Distinct());

            var associatedList = _custEvidenceService.GetAssociatedAccountByCTSCustIds(ctsCustIds);

            if (associatedList.Any())
            {
                var custIds = string.Join(",", associatedList.Distinct());

                var dbResult = _custEvidenceService.UpdateBetLimitOfFlaggedCustomer(custIds);

                if (dbResult.ErrorCode != 0)
                {
                    WriteLogError("UpdateBetLimitOfFlaggedCustomer", dbResult.ErrorMsg, custEvidences);
                }
            }

            return this;
        }

        public IProcessingService UpdateProcessedCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            var objects = custEvidences.Select(x => new { x.ID });

            var jsonData = JsonConvert.SerializeObject(objects);

            var starixITId = 10278938;

            var dbResult = _custEvidenceService.UpdateProcessedCustEvidence(jsonData, starixITId);

            if (dbResult.ErrorCode != 0)
            {
                WriteLogError("UpdateProcessedCustEvidence", dbResult.ErrorMsg, custEvidences);
            }

            return this;
        }

        public List<CustomerEvidenceFailedToProcessModel> GetLogError()
        {
            return _failedToProcessList;
        }

        private void WriteLogError(string method, string errorMessage, IEnumerable<CustomerEvidenceFromFileModel> errorCustEvidences)
        {
            _failedToProcessList.AddRange(
                errorCustEvidences.Select(x => new CustomerEvidenceFailedToProcessModel
                {
                    FileKey = x.FileKey,
                    SubscriberID = x.SubscriberID,
                    CTSCustID = x.CTSCustID,
                    EvidenceID = x.EvidenceID,
                    ErrorContent = method + " " + errorMessage
                })
            );
        }
       
    }
}
