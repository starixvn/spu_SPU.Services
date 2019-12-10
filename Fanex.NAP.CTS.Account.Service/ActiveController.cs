namespace Fanex.NAP.CTS.Account.Service
{
    using EasyNetQ;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Model;
    using Fanex.NAP.SPU.Queue;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class ActiveController : ApiController
    {
        private readonly IDataCenterService dataCenterService;
       // private EasyNetQConnector easyNetQService;

        public ActiveController()
        {
            dataCenterService = new DataCenterService();

            //easyNetQService = new EasyNetQConnector(AppSettings.RabbitMQ_Host,
            //    AppSettings.RabbitMQ_VirtualHost,
            //    Convert.ToInt32(AppSettings.RabbitMQ_PrefetchCount),
            //    AppSettings.RabbitMQ_UserName,
            //    AppSettings.RabbitMQ_Password).CreateBus();

            //easyNetQService.DeclareQueue(AppSettings.RabbitMQ_QueueName)
            //                .DeclareExchange(AppSettings.RabbitMQ_Exchange)
            //                .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
            //                .Bind();
        }

        public bool Post()
        {
            var requestContent = Request.Content;
            var jsonContent = requestContent.ReadAsStringAsync().Result;

            if (!string.IsNullOrWhiteSpace(jsonContent))
            {
                var result = JsonConvert.DeserializeObject<List<RawAccountModel>>(jsonContent);

                return Process(result);
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        private bool Process(List<RawAccountModel> accounts)
        {
            try
            {
                if (accounts != null && accounts.Any())
                {
                    var accountJson = JsonConvert.SerializeObject(accounts);
                    LogService.Info(accountJson);

                    var insertedAccounts = dataCenterService.TransformDCSAccount(accounts);
                                        
                    if (insertedAccounts != null && insertedAccounts.Any())
                    {                        
                        dataCenterService.TransformAccountCompleted(insertedAccounts);

                       // TransformToGraphDB(insertedAccount);
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

        //private void TransformToGraphDB(List<RawAccountModel> accounts)
        //{
        //    foreach (var item in accounts)
        //    {
        //        var data = new AssociationModelForGraph()
        //        {
        //            Username2 = item.LoginNameWithPrefix,
        //            SubscriberId = item.SubscriberId
        //        };

        //        var message = new Message<AssociationModelForGraph>(data);
        //        easyNetQService.AdvanceBus.PublishAsync(easyNetQService.Exchange, easyNetQService.RoutingKey, true, message);
        //    }

        //    easyNetQService.Disposed();
        //}
    }
}
