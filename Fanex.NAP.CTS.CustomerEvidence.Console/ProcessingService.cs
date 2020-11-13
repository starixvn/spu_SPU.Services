namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Newtonsoft.Json;

    public class ProcessingService : IProcessingService
    {
        private ICustomerEvidenceService _custEvidenceService;

        public ProcessingService()
        {
            _custEvidenceService = new CustomerEvidenceService();
        }

        public bool RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            var ctsCustIdList = custEvidences.Select(x => new { x.CTSCustID }).Distinct();

            var jsonData = JsonConvert.SerializeObject(ctsCustIdList);

            var sessionId = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            var dbResult = _custEvidenceService.RemoveCustomerEvidenceByCTSCustIds(jsonData, sessionId, false, false);

            return dbResult.ErrorCode == 0;
        }

        public bool InsertNewCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences, bool isOverwrite, bool isPrevSuccess)
        {
            if (isPrevSuccess)
            {
                var objects = custEvidences.Select(x => new
                {
                    x.CTSCustID,
                    x.EvidenceID,
                    x.Remark
                });

                var jsonData = JsonConvert.SerializeObject(objects);

                var starixITId = 10278938;

                var dbResult = _custEvidenceService.InsertCustomerEvidenceMuitiple(jsonData, starixITId, isOverwrite);

                return dbResult.ErrorCode == 0;
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

                return dbResult.ErrorCode == 0;
            }

            return isPrevSuccess;
        }

        public IProcessingService SyncCustomerClassToMainDb(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            var ctsCustIdList = custEvidences.Select(x => new { x.CTSCustID }).Distinct();

            var ctsCustIds = string.Join(",", ctsCustIdList.Distinct().ToList());

            var dbResult = _custEvidenceService.SyncCustomerClassToMainDb(ctsCustIds);            

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
            }

            return this;
        }

        public IProcessingService UpdateProcessedCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            var objects = custEvidences.Select(x => new { x.ID });

            var jsonData = JsonConvert.SerializeObject(objects);

            var starixITId = 10278938;

            var dbResult = _custEvidenceService.UpdateProcessedCustEvidence(jsonData, starixITId);

            return this;
        }

    }
}
