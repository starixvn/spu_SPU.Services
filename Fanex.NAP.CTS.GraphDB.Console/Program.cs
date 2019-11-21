using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Sentry.RavenClient;
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

            AddHandlerForGetMassage(InsertCustomer);
        }

        private static void UpdateData(string key, AssociationModel criteria)
        {
            var result = DbSessionFactory.Execute(key, criteria);

            if (result != null)
            {
                if (associations.Count < SPU.Core.AppSettings.TransBatch)
                {
                    associations.Add(new IsSynAssociationModel
                    {
                        AssociationId = criteria.AssociationId,
                        SyncGraphDB = 2
                    });
                }
                else
                {
                    var xmlData = DCS.Core.Utilities.XMLConvert(associations);

                    dataCenterService.UpdateIsSyncByAssociationId(xmlData);

                    associations.Clear();
                }
            }
        }

        private static void InsertCustomer(AssociationModel criteria)
        {
            // remove test account
            if (!criteria.Username.Contains("test") || criteria.Username2 == null || !criteria.Username2.Contains("test"))
            {
                try
                {
                    var custId = DbSessionFactory.Query<int>(FPS.GetCustomerById, new { criteria.CustId });
                    var deviceId = DbSessionFactory.Query<int>(FPS.GetDeviceById, new { criteria.DeviceId });
                    var association = DbSessionFactory.Query<int>(FPS.CheckExistedAssociation, new { criteria.CustId, criteria.DeviceId });

                    if (criteria.DeviceId == 0)
                    {
                        if (!custId.Any())
                        {
                            UpdateData(FPS.AddNewCustomer, criteria);
                        }
                    }
                    else
                    {
                        UpdateData(FPS.AddAssociation, criteria);

                        if (custId.Any())
                        {
                            if (deviceId.Any())
                            {
                                // case old account, old device
                                if (association.Any())
                                {
                                    UpdateData(FPS.UpdateCustomerUsername2, criteria);
                                }
                                else
                                {
                                    UpdateData(FPS.MergeGroup, criteria);
                                }
                            }
                        }
                        else
                        {
                            if (deviceId.Any())
                            {
                                // case new account, old device
                                UpdateData(FPS.AddCustomerGroupRelationship, criteria);
                            }
                            else
                            {
                                // case new account, old device
                                UpdateData(FPS.CreateNewGroup, criteria);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                     .ToString();

                    LogService.Error(logMessage);
                }
            }
        }

        private static void AddHandlerForGetMassage(Action<AssociationModel> handler)
        {
            easyNetQService.AdvanceBus.Consume(easyNetQService.Queue,
                registration => registration.Add<AssociationModel>((message, info) =>
                {
                    handler(message.Body);
                }));
        }
    }
}
