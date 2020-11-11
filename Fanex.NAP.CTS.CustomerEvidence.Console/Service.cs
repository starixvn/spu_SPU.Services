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
        private ICustomerEvidenceService _custEvidenceService;
        private IProcessingService _processingService;
        
        public Service()
        {
            _custEvidenceService = new CustomerEvidenceService();
            _processingService = new ProcessingService();            
        }

        public void ProcessCustEvidenceFromFile()
        {
            try
            {
                var custEvidenceFromFiles = _custEvidenceService.GetCustomerEvidenceFromFile();

                if (custEvidenceFromFiles != null && custEvidenceFromFiles.Any())
                {
                    var groups = custEvidenceFromFiles.GroupBy(x => x.FileKey);

                    foreach (var custEviList in groups)
                    {
                        var isOverwrite = custEviList.ToList()[0].IsOverwrite;

                        _processingService.ResetOverwrite(isOverwrite);

                        InsertCustEvidence(custEviList);
                    }
                }
            }
            catch (Exception ex)
            {
                //LogService.Info($"CustomerEvidence Service ProcessCustEvidenceFromFile exception: {ex.Message}", ex);
            }
        }

        private void InsertCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidenceInFiles)
        {
            custEvidenceInFiles = custEvidenceInFiles.OrderBy(x => x.CTSCustID);

            var loopCustEvidenceList = ChunkSize(custEvidenceInFiles.ToList());

            foreach (var custEvidences in loopCustEvidenceList)
            {
                try
                {
                    _processingService.ResetPrevious();

                    _processingService
                        .RemoveOldEvidence(custEvidences)
                        .InsertNewCustEvidence(custEvidences)
                        .SyncCustomerClassToMainDb(custEvidences)
                        .UpdateBetLimitOfFlaggedCustomer(custEvidences)
                        .UpdateProcessedCustEvidence(custEvidences);
                }
                catch (Exception ex)
                {
                    //LogService.Info($"CustomerEvidence Service InsertCustEvidence exception: {ex.Message}", ex);
                }
            }
        }
        
        private IEnumerable<IEnumerable<CustomerEvidenceFromFileModel>> ChunkSize(List<CustomerEvidenceFromFileModel> custEvidenceInFiles)
        {
            var size = AppSettings.CustEvidenceFromFileChunkSize;

            var queue = new Queue<CustomerEvidenceFromFileModel>();
            var totalItem = custEvidenceInFiles.Count();

            for (int index = 0; index < totalItem; index++)
            {
                if (queue.Count >= size)
                {
                    var prevIndex = index - 1;

                    if (custEvidenceInFiles[index].CTSCustID == custEvidenceInFiles[prevIndex].CTSCustID)
                    {
                        queue.Enqueue(custEvidenceInFiles[index]);

                        continue;
                    }

                    var ret = queue;
                    queue = new Queue<CustomerEvidenceFromFileModel>();

                    yield return ret;
                }

                queue.Enqueue(custEvidenceInFiles[index]);
            }

            yield return queue;           
        }
        
    }
}
