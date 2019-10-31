namespace Fanex.NAP.FPS.Renovation.DataTransform.Device.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FPS.Renovation.Core;
    using Fanex.NAP.FPS.Renovation.DataTransform.Device.Service.Services.Implement;
    using Fanex.NAP.FPS.Renovation.DataTransform.Device.Service.Services.Interface;
    using Fanex.NAP.FPS.Renovation.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
                var result = JsonConvert.DeserializeObject<List<DevicePackage>>(jsonContent);

                return Process(result);
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        private bool Process(List<DevicePackage> data)
        {
            try
            {
                if (data != null && data.Any())
                {
                    Core.Utilities.WriteLog("InsertDevice.log", "Start: " + DateTime.Now);

                    var dataBatchs = data.Chunk(100);

                    Parallel.ForEach(dataBatchs, x =>
                    {
                        dataCenterService.InsertDevice(x.ToList());

                        Console.WriteLine($"Device is inserted ({x.Count()})");
                    });

                    Core.Utilities.WriteLog("InsertDevice.log", "End: " + DateTime.Now);                    

                    return true;
                }
            }
            catch (Exception ex)
            {                
                if (!ex.InnerException.Message.Contains("Deadlock found when trying to get lock"))
                {
                    var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                       .ToString();

                    LogService.Error(logMessage);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }
    }
}
