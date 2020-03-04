namespace Fanex.NAP.DCS.Transaction.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.Transaction.Service.Services.Implement;
    using Fanex.NAP.DCS.Transaction.Service.Services.Interface;
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
                    
                    dataTransformService
                    .Initial(model.MinTransId, model.MaxTransId)
                    .UnhandleSubscriberAccountProcessing()
                    .UserAgentProcessing()
                    .TransactionProcessing();                                                 

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
