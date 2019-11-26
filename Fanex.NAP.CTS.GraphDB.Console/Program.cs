using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Model.GraphDB;
using Fanex.NAP.CTS.Renovation.GraphDB.Console.Services.Implement;
using Fanex.NAP.CTS.Renovation.GraphDB.Console.Services.Interface;
using Fanex.NAP.SPU.Graph.Core;
using Fanex.NAP.SPU.Graph.Core.Neo4j;
using Fanex.NAP.SPU.Model;
using Fanex.NAP.SPU.Queue;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.Renovation.GraphDB.Console
{
    class Program
    {
        private static EasyNetQConnector easyNetQService;
        private static IDataCenterService dataCenterService;
        private static List<IsSynAssociationModel> associations;

        static void Main(string[] args)
        {
            dataCenterService = new DataCenterService();
            associations = new List<IsSynAssociationModel>();

            DbSessionFactory.Init()
                .RegisterConfigs("DB.MiddleService")
                .RegisterXmlFile("~/App_Data")
                .RegisterDriverFactory(new Neo4jDriverFactory())
                .RegisterCommandBuilder(new Neo4jCommandBuilder("~/App_Data/scripts"))
                .RegisterCommandExecutor(new Neo4jExecutor())
                .Run();

            LogManager
                .SetDefaultLogCategory(SPU.Core.AppSettings.FanexLogCategory)
                .Use(new List<ILogEngine>
                {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(SPU.Core.AppSettings.SentryLogging)
                    })
                });

            DbSettingProviderManager
                .StartNewSession()
                .WithMySql()
                .UseDefaultDbSettingProvider(resourcePath: SPU.Core.AppSettings.AppDataSetting)
                .UseDefaultConnectionStringProvider()
                .Run();

            easyNetQService = new EasyNetQConnector(AppSettings.RabbitMQ_Host,
                AppSettings.RabbitMQ_VirtualHost,
                Convert.ToInt32(AppSettings.RabbitMQ_PrefetchCount),
                AppSettings.RabbitMQ_Username,
                AppSettings.RabbitMQ_Password).CreateBus();

            easyNetQService.DeclareQueue(AppSettings.RabbitMQ_Queue)
                            .DeclareExchange(AppSettings.RabbitMQ_Exchange)
                            .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
                            .Bind();

            AddHandlerForGetMassage(TransformFromCTS);
        }

        private static void TransformFromCTS(AssociationModelForGraph criteria)
        {
            if (criteria.CustId == 0 && !string.IsNullOrEmpty(criteria.Username2) && criteria.SubscriberId > 0)
            {
                DbSessionFactory.Execute(CTS.TransformFromDCS, criteria);
            }
            else if (criteria.DeviceId > 0)
            {
                DbSessionFactory.Execute(CTS.TransformFromDCS, criteria);
            }
            else
            {
                DbSessionFactory.Execute(CTS.TransformFromDCS, criteria);
            }
        }

        private static void AddHandlerForGetMassage(Action<AssociationModelForGraph> handler)
        {
            easyNetQService.AdvanceBus.Consume(easyNetQService.Queue,
                registration => registration.Add<AssociationModelForGraph>((message, info) =>
                {
                    handler(message.Body);
                }));
        }
    }
}
