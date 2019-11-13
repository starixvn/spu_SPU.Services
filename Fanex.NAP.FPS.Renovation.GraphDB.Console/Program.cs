using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Graph.Data;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.FPS.Renovation.GraphDB.Console.Services.Implement;
using Fanex.NAP.FPS.Renovation.GraphDB.Console.Services.Interface;
using Fanex.NAP.FPS.Renovation.Model;
using Fanex.NAP.FPS.Renovation.Queue;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.GraphDB.Console
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
                .SetDefaultLogCategory(Core.AppSettings.FanexLogCategory)
                .Use(new List<ILogEngine>
                {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(Core.AppSettings.SentryLogging)
                    })
                });

            DbSettingProviderManager
                .StartNewSession()
                .WithMySql()
                .UseDefaultDbSettingProvider(resourcePath: Core.AppSettings.AppDataSetting)
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

        private static void AddHandlerForGetMassage(Action<AssociationModel> handler)
        {
            easyNetQService.AdvanceBus.Consume(easyNetQService.Queue,
                registration => registration.Add<AssociationModel>((message, info) =>
                {
                    handler(message.Body);
                }));
        }

        private static void UpdateData(string key, AssociationModel criteria)
        {
            var result = DbSessionFactory.Execute(key, criteria);

            if (result != null)
            {
                if (associations.Count < Core.AppSettings.TransBatch)
                {
                    associations.Add(new IsSynAssociationModel
                    {
                        AssociationId = criteria.AssociationId,
                        SyncGraphDB = 2
                    });
                }
                else
                {
                    var xmlData = Core.Utilities.XMLConvert(associations);

                    dataCenterService.UpdateIsSyncByAssociationId(xmlData);

                    associations.Clear();
                }
            }
        }

        private static void InsertCustomer(AssociationModel criteria)
        {
            // ko luu SMA, chi luu member, roleid = 1
            if (criteria.RoleId == 1)
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
                                    // case acc cu, device cu
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
                                    // case acc moi, device cu
                                    UpdateData(FPS.AddCustomerGroupRelationship, criteria);
                                }
                                else
                                {
                                    // case acc moi, device moi
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
        }
    }
}
