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
    using System.Threading;

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
                    LogService.Info("Service Autotag Normal - start!");

                    newClassification = _dataCenterService.GetNewClassification();

                    LogService.Info($"Service Autotag Normal - Get New Classification:{JsonConvert.SerializeObject(newClassification)}");

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
            if (generalClassification != null && generalClassification.Any())
            {
                var custGeneralCategoryForMainDB = new List<CustomerGeneralClassificationModel>();
                var newGeneralClassificationChunk = generalClassification.Chunk(AppSettings.BatchSize).ToList();

                foreach (var listClassification in newGeneralClassificationChunk)
                {
                    try
                    {
                        var jsonClassification = JsonConvert.SerializeObject(listClassification);

                        // 1. Insert data in CTS
                        var typeCustModel = _dataCenterService.InsertMainCustomerClassification(jsonClassification);

                        LogService.Info($"Service Autotag Normal - Insert CTS Complete!:{JsonConvert.SerializeObject(jsonClassification)} - Output: {JsonConvert.SerializeObject(typeCustModel)}" );

                        if (string.IsNullOrEmpty(typeCustModel.ErrorMessage))
                        {
                            var custIdList = string.Join(",", listClassification.Select(c => c.CustId).Distinct());

                            // 2. Update IsApplied in mainDB
                            _dataCenterService.UpdateAppliedStatusClassification(custIdList, true);

                            // 3. Get lastest classification in CTS
                            var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",", typeCustModel.CustIds));

                            LogService.Info($"Service Autotag Normal - Get latest Complete!:{JsonConvert.SerializeObject(lastestClassifications)}");

                            if (lastestClassifications != null && lastestClassifications.Any())
                            {
                                // 4. Update CustomerClass in table custinfo to MainDB
                                var isSuccessToMain = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, 2);

                                if (isSuccessToMain)
                                {
                                    var updateApplyCCs = lastestClassifications.Select(x => new { x.CustId, x.ScannedTime });

                                    var jsonUpdateApplyCCs = JsonConvert.SerializeObject(updateApplyCCs);

                                    // 5. Update IsAppliedCC = 1 in CTS
                                    _dataCenterService.UpdateAppliedCustomerClassification(jsonUpdateApplyCCs);

                                    LogService.Info("Service Autotag Normal - Insert MainDB Complete!");
                                }

                                // 6. Push CC to Soon88
                                _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);

                                LogService.Info("Service Autotag Normal - FeedSoon88 Complete!");
                            }
                        }
                        else
                        {
                            LogService.Error($"CustomerClassification.AutoTag Service - InsertGeneralClassificationCusts - InsertMainCustomerClassification error:  {typeCustModel.ErrorMessage}");
                        }                        
                    }
                    catch (Exception ex)
                    {
                        LogService.Error($"CustomerClassification.AutoTag Service - InsertGeneralClassificationCusts exception {ex.Message}", ex);
                    }
                }
            }
        }
    }
}
