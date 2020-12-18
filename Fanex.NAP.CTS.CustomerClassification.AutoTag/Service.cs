namespace Fanex.NAP.CTS.CustomerClassification.AutoTag
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Service
    {
        private IDataCenterService _dataCenterService;
        private IPushDataToExternalService _pushDataToExternalService;

        public Service()
        {
            _dataCenterService = new DataCenterService();
            _pushDataToExternalService = new PushDataToExternalService();
        }

        public void Process()
        {
            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);

            try
            {

                var newClassification = new List<CustomerClassificationModelMoreInfo>();

                int insertedRows = 0;

                do
                {
                    newClassification = _dataCenterService.GetNewClassification();

                    var generalClassifications = newClassification?.Where(c => c.IsGeneralCategory);

                    InsertGeneralClassificationCusts(generalClassifications);

                } while (newClassification != null && newClassification.Any());

                insertedRows += newClassification.Count;
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        private void InsertGeneralClassificationCusts(IEnumerable<CustomerClassificationModelMoreInfo> generalClassification)
        {
            if (generalClassification == null && generalClassification.Any())
            {
                var custGeneralCategoryForMainDB = new List<CustomerGeneralClassificationModel>();
                var newGeneralClassificationChunk = generalClassification.Chunk(AppSettings.BatchSize).ToList();

                foreach (var listClassification in newGeneralClassificationChunk)
                {
                    try
                    {
                        var jsonData = JsonConvert.SerializeObject(listClassification);

                        // Insert data in CTS
                        var typeCustModel = _dataCenterService.InsertMainCustomerClassification(jsonData);

                        var custIdList = string.Join(",", listClassification.Select(c => c.CustId).Distinct());

                        // Update IsApplied in mainDB
                        _dataCenterService.UpdateAppliedStatusClassification(custIdList, true);

                        // Get lastest classification in CTS
                        var lastestClassifications = _dataCenterService.GetLatestClassification(typeCustModel);

                        // Update applied classification
                        if (lastestClassifications != null && lastestClassifications.Any())
                        {
                            // Update CustomerClass in table custinfo to MainDB
                            _dataCenterService.UpdateCustomerGeneralClassificationtoMainDB(lastestClassifications);

                            var updateApplyCCs = lastestClassifications.Select(x => new { x.CustId, x.ScannedTime });

                            jsonData = JsonConvert.SerializeObject(updateApplyCCs);

                            // Update IsAppliedCC = 1 in CTS
                            _dataCenterService.UpdateAppliedCustomerClassification(jsonData);
                        }

                        // Feed data to Soon88
                        //FeedDataToSoon88Helper.FeedCCToSoon88(lastestClassifications);

                        _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
        }
    }
}
