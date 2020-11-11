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
        public bool IsPrevSuccess;
        public bool IsOverwrite;

        public ProcessingService()
        {
            _custEvidenceService = new CustomerEvidenceService();
        }

        public void ResetOverwrite(bool isOverwrite)
        {
            this.IsOverwrite = isOverwrite;
        }

        public void ResetPrevious()
        {
            this.IsPrevSuccess = true;           
        }

        public IProcessingService RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            if (this.IsPrevSuccess && this.IsOverwrite)
            {
                var ctsCustIdList = custEvidences.Select(x => new { x.CTSCustID }).Distinct();

                var jsonData = JsonConvert.SerializeObject(ctsCustIdList);

                var sessionId = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                var dbResult = _custEvidenceService.RemoveCustomerEvidenceByCTSCustIds(jsonData, sessionId, false, false);

                this.IsPrevSuccess = dbResult.ErrorCode == 0;
            }

            return this;
        }

        public IProcessingService InsertNewCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            if (this.IsPrevSuccess)
            {
                var objects = custEvidences.Select(x => new
                {
                    x.CTSCustID,
                    x.EvidenceID,
                    x.Remark
                });

                var jsonData = JsonConvert.SerializeObject(objects);

                var starixITId = 10278938;

                var dbResult = _custEvidenceService.InsertCustomerEvidenceMuitiple(jsonData, starixITId, this.IsOverwrite);

                this.IsPrevSuccess = dbResult.ErrorCode == 0;
            }

            return this;
        }

        public IProcessingService SyncCustomerClassToMainDb(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            if (this.IsPrevSuccess)
            {
                var ctsCustIdList = custEvidences.Select(x => new { x.CTSCustID }).Distinct();

                var ctsCustIds = string.Join(",", ctsCustIdList.Distinct().ToList());

                var dbResult = _custEvidenceService.SyncCustomerClassToMainDb(ctsCustIds);

                this.IsPrevSuccess = dbResult.ErrorCode == 0;
            }

            return this;
        }

        public IProcessingService UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            if (this.IsPrevSuccess)
            {
                var ctsCustIds = string.Join(",", custEvidences.Select(x => x.CTSCustID).Distinct());

                var associatedList = _custEvidenceService.GetAssociatedAccountByCTSCustIds(ctsCustIds);

                if (associatedList.Any())
                {
                    var custIds = string.Join(",", associatedList.Distinct());

                    var dbResult = _custEvidenceService.UpdateBetLimitOfFlaggedCustomer(custIds);

                    this.IsPrevSuccess = dbResult.ErrorCode == 0;
                }
            }

            return this;
        }

        public IProcessingService UpdateProcessedCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidences)
        {
            if (this.IsPrevSuccess)
            {
                var objects = custEvidences.Select(x => new { x.ID });

                var jsonData = JsonConvert.SerializeObject(objects);

                var starixITId = 10278938;

                var dbResult = _custEvidenceService.UpdateProcessedCustEvidence(jsonData, starixITId);

                this.IsPrevSuccess = dbResult.ErrorCode == 0;
            }

            return this;
        }

    }
}
