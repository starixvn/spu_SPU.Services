namespace Fanex.NAP.CTS.AssociationByIP.Service
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
                var result = JsonConvert.DeserializeObject<List<RawTransactionByIpModel>>(jsonContent);


                LogService.Error(JsonConvert.SerializeObject(result));

                return Process(jsonContent);
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        private bool Process(string transactionByIpJson)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(transactionByIpJson))
                {
                    dataCenterService.TransformDCSTransactionIPToCTSAssociationByIP(transactionByIpJson);

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
