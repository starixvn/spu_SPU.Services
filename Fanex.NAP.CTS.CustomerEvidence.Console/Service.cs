namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;    
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;    
    using System.Linq;

    public class Service
    {
        private ICustomerEvidenceService custEvidenceService;

        public Service()
        {
            custEvidenceService = new CustomerEvidenceService();
        }

        public void ProcessCustEvidenceFromFile()
        {
            try
            {
                var custEvidenceFromFiles = custEvidenceService.GetCustomerEvidenceFromFile();

                if (custEvidenceFromFiles != null && custEvidenceFromFiles.Any())
                {
                    /* 
                     * 1 Insert Cust Evidence
                     *  1.1 Group for file
                     *  1.2 Split batch and insert
                     *  1.3 Update IsProcessd = 1
                     * 2 Update Bet Limit
                     */

                    var groups = custEvidenceFromFiles.GroupBy(x => x.FileKey);

                    foreach (var custEviList in groups)
                    {
                        var isOverwrite = custEviList.ToList()[0].IsOverwrite;

                        InsertCustEvidence(custEviList, isOverwrite);

                    }
                }
            }
            catch (Exception ex)
            {
                //LogService.Info($"CustomerEvidence Service ProcessCustEvidenceFromFile exception: {ex.Message}", ex);
            }
        }

        private void InsertCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidenceFromFiles, bool isOverwrite)
        {
            var loopCustEvidenceList = custEvidenceFromFiles.ChunkSize(AppSettings.CustEvidenceFromFileChunkSize);

            foreach (var custEvidences in loopCustEvidenceList)
            {
                try
                {
                    var isSuccess = true;
                    var custEvidenceIds = string.Join(",", custEvidenceFromFiles.Select(x => new { x.ID }));
                    var ctsCustIdList = custEvidenceFromFiles.Select(x => new { x.CTSCustID }).Distinct();
                    var ctsCustIds = string.Join(",", ctsCustIdList.Distinct().ToList());
                    var jsonData = JsonConvert.SerializeObject(ctsCustIdList);

                    if (isOverwrite)
                    {
                        isSuccess = RemoveOldEvidence(jsonData);
                    }

                    if (isSuccess)
                    {
                        isSuccess = InsertNewCustEvidence(jsonData, isOverwrite);
                    }

                    if (isSuccess)
                    {
                        UpdateProcessCustEvidence(custEvidenceIds);
                        SyncCustomerClassToMainDb(ctsCustIds);
                    }
                }
                catch (Exception ex)
                {
                    //LogService.Info($"CustomerEvidence Service InsertCustEvidence exception: {ex.Message}", ex);
                }
            }
        }

        private bool RemoveOldEvidence(string jsonData)
        {
            var sessionId = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            var dbResult = custEvidenceService.RemoveCustomerEvidenceByCTSCustIds(jsonData, sessionId, false, false);

            return dbResult.ErrorCode == 0;
        }

        private bool InsertNewCustEvidence(string jsonData, bool isOverwrite)
        {
            var starixITId = 10278938;
            
            var dbResult = custEvidenceService.InsertCustomerEvidenceMuitiple(jsonData, starixITId, isOverwrite);

            return dbResult.ErrorCode == 0;
        }

        private bool SyncCustomerClassToMainDb(string ctsCustIds)
        {
            var dbResult = custEvidenceService.SyncCustomerClassToMainDb(ctsCustIds);

            return dbResult.ErrorCode == 0;
        }

        private bool UpdateProcessCustEvidence(string custEvidenceIds)
        {
            var starixITId = 10278938;

            var dbResult = custEvidenceService.UpdateProcessCustEvidence(custEvidenceIds, starixITId);

            return dbResult.ErrorCode == 0;
        }

        private bool UpdateBetLimitOfFlaggedCustomer(string custIds)
        {
            var dbResult = custEvidenceService.UpdateBetLimitOfFlaggedCustomer(custIds);

            return dbResult.ErrorCode == 0;
        }
    }
}
