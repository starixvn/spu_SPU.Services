namespace Fanex.NAP.DCS.Device.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Device.Service.Services.Implement;
    using Fanex.NAP.DCS.Device.Service.Services.Interface;
    using System;
    using System.Web.Http;

    public class ActiveController : ApiController
    {
        private readonly IDataCenterService dataCenterService;

        public ActiveController()
        {
            dataCenterService = new DataCenterService();
        }

        public bool Post()
        {
            var requestContent = Request.Content;
            var jsonContent = requestContent.ReadAsStringAsync().Result;

            if (!string.IsNullOrWhiteSpace(jsonContent))
            {                
                return Process(jsonContent);
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        private bool Process(string jsonData)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(jsonData))
                {                                 
                    dataCenterService.InsertDevice(jsonData);
                    LogService.Info($"DCS - Tranform - Device: {jsonData}");

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
