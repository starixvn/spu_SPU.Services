using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.FPS.Renovation.DataTransform.Transaction.Service.Services.Implement;
using Fanex.NAP.FPS.Renovation.DataTransform.Transaction.Service.Services.Interface;
using Fanex.NAP.FPS.Renovation.Model;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Web.Http;

namespace Fanex.NAP.FPS.Renovation.DataTransform.Transaction.Service
{
    public class ActiveController : ApiController
    {
        private readonly IDataTransformService dataTransformService;

        public ActiveController()
        {
            dataTransformService = new DataTransformService();
        }

        public bool Post()
        {
            var requestContent = Request.Content;
            var jsonContent = requestContent.ReadAsStringAsync().Result;

            if (!string.IsNullOrWhiteSpace(jsonContent))
            {
                var result = JsonConvert.DeserializeObject<StagingRawTransactionModel>(jsonContent);

                return Process(result);
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        private bool Process(StagingRawTransactionModel model)
        {
            try
            {
                if (model.MinStagingTransId != 0 && model.MaxStagingTransId != 0)
                {
                    Core.Utilities.WriteLog("InsertTransaction.log", "Start: " + DateTime.Now);

                    Console.WriteLine("============> Min ID:" + model.MinStagingTransId + " - Max ID:" + model.MaxStagingTransId + " <================");

                    dataTransformService
                    .Initial(model.MinStagingTransId, model.MaxStagingTransId)
                    .UnhandleSubscriberAccountProcessing()
                    .UserAgentProcessing()
                    .AccountProcessing()
                    .TransactionProcessing()              
                    .CompleteProcessing();

                    Core.Utilities.WriteLog("InsertTransaction.log", "End: " + DateTime.Now);

                    return true;
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                LogService.Error(logMessage);
            }

            return false;
        }
    }
}
