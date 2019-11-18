namespace Fanex.NAP.DCS.DataTransform.Transaction.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.DataTransform.Transaction.Service.Services.Implement;
    using Fanex.NAP.DCS.DataTransform.Transaction.Service.Services.Interface;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Web.Http;

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
                if (model.MinTransId != 0 && model.MaxTransId != 0)
                {
                    Utilities.WriteLog("InsertTransaction.log", "Start: " + DateTime.Now);

                    Console.WriteLine("============> Min ID:" + model.MinTransId + " - Max ID:" + model.MaxTransId + " <================");

                    dataTransformService
                    .Initial(model.MinTransId, model.MaxTransId)
                    .UnhandleSubscriberAccountProcessing()
                    .UserAgentProcessing()
                    .TransactionProcessing();           
                    
                    Utilities.WriteLog("InsertTransaction.log", "End: " + DateTime.Now);

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
