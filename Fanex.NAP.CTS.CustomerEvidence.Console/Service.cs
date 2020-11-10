namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class Service
    {
        private IDataCenterService dataService;

        public Service()
        {
            dataService = new DataCenterService();
        }

        public void ProcessCustEvidenceFromFile()
        {
            try
            {
                
                var custEvidenceFromFiles = dataService.GetCustomerEvidenceFromFile();

                if (custEvidenceFromFiles != null && custEvidenceFromFiles.Any())
                {
                    /* 
                     * 1 Insert Cust Evidence
                     *  1.1 Group for file
                     *  1.2 Split batch and insert
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
                //LogService.Info($"ProcessCustEvidenceFromFile ({AppSettings.EnvironmentName}) - exception: {ex.Message}", ex);
            }
        }

        private void InsertCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidenceFromFiles, bool isOverwrite)
        {
            var loopCustEvidenceList = custEvidenceFromFiles.Chunk(AppSettings.CustEvidenceFromFileChunkSize);

            foreach (var custEvidences in loopCustEvidenceList)
            {
                try
                {
                    var isCanInsert = true;

                    if (isOverwrite)
                    {
                        isCanInsert = RemoveOldEvidence(custEvidences);
                    }

                    if (isCanInsert)
                    {
                        InsertNewCustEvidence(custEvidences);
                    }
                }
                catch (Exception ex)
                {
                    //LogService.Info($"InsertCustEvidence ({AppSettings.EnvironmentName}) - exception: {ex.Message}", ex);
                }
            }
        }

        private bool RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidenceFromFiles)
        {
            return true;
        }

        private void InsertNewCustEvidence(IEnumerable<CustomerEvidenceFromFileModel> custEvidenceFromFiles)
        {

        }
    }
}
