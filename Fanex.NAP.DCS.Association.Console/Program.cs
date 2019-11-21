using EasyNetQ;
using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.DCS.Services.Implement;
using Fanex.NAP.SPU.Core;
using Fanex.NAP.SPU.Model;
using Fanex.NAP.SPU.Queue;
using Newtonsoft.Json;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.DCS.Association.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            DbSettingProviderManager
                .StartNewSession()
                .WithMySql()                
                .UseDefaultDbSettingProvider(resourcePath: AppSettings.AppDataSetting)
                .UseDefaultConnectionStringProvider()
                .Run();

            LogManager
               .SetDefaultLogCategory(AppSettings.FanexLogCategory)
               .Use(new List<ILogEngine>
               {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(AppSettings.SentryLogging)
                    })
               });

            var dataCenterService = new DataCenterService();

           var _rabbitMQ = new EasyNetQConnector(AppSettings.RabbitMQ_Host + ":" + AppSettings.RabbitMQ_Post,
                AppSettings.RabbitMQ_VirtualHost,
                AppSettings.RabbitMQ_PrefetchCount,
                AppSettings.RabbitMQ_UserName,
                AppSettings.RabbitMQ_Password);

            _rabbitMQ.CreateBus()
                .DeclareExchange(AppSettings.RabbitMQ_Exchange)
                .DeclareQueue(AppSettings.RabbitMQ_QueueName)
                .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
                .Bind();

            try
            {
                var associations = dataCenterService.GetAssociationForGrapDB(0, AppSettings.SizeOfAssociation);

                if (associations != null && associations.Any())
                {
                    foreach (var association in associations)
                    {
                        var messageQueue = new Message<AssociationModel>(association, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                        _rabbitMQ.AdvanceBus.PublishAsync(_rabbitMQ.Exchange, _rabbitMQ.RoutingKey, false, messageQueue);
                    }

                    var isSynAssociation = associations.Select(x => new IsSynAssociationModel
                    {
                        AssociationId = x.AssociationId,
                        SyncGraphDB = 1
                    }).ToList();

                    var jsonData = JsonConvert.SerializeObject(isSynAssociation);

                    dataCenterService.UpdateIsSyncByAssociationId(jsonData);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
            finally
            {
                _rabbitMQ.Disposed();
            }
        }
    }
}
