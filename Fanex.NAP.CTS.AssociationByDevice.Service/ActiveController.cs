namespace Fanex.NAP.CTS.AssociationByDevice.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
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
                var result = JsonConvert.DeserializeObject<List<RawAssociationModel>>(jsonContent);

                return Process(result);
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        private bool Process(List<RawAssociationModel> associations)
        {
            try
            {
                if (associations != null && associations.Any())
                {
                    var associationByDevice = dataCenterService.TransformAssociationByDevice(associations);

                    if (associationByDevice != null && associationByDevice.Any())
                    {
                        dataCenterService.TransformAssociationCompleted(associationByDevice);
                    }

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
