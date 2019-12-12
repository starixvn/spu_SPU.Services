namespace Fanex.NAP.DCS.Device.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Core;
    using Fanex.NAP.DCS.Device.Service.Services.Implement;
    using Fanex.NAP.DCS.Device.Service.Services.Interface;
    using Fanex.NAP.DCS.Model;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
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
                var result = JsonConvert.DeserializeObject<TransformDevicePackageModel>(jsonContent);

                return Process(result);
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        private bool Process(TransformDevicePackageModel data)
        {
            try
            {
                if (data.Devices != null && data.Devices.Any())
                {                                 
                    dataCenterService.InsertDevice(data.Devices);                                        

                    return true;
                }
            }
            catch (Exception ex)
            {
                //if (!ex.InnerException.Message.Contains("Deadlock found when trying to get lock"))
                //{
                //    var logMessage = new LogMessage(ex.Message)
                //       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                //       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                //       .ToString();

                //    LogService.Error(logMessage);
                //}
                //else
                //{
                //    Console.WriteLine(ex.Message);
                //}

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
