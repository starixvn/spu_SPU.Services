using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Graph.Data;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;

namespace Fanex.NAP.FPS.Renovation.GraphDB.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
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

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Runner()
            };

            if (Environment.UserInteractive)
            {
                // run console
                var type = typeof(ServiceBase);
                const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var method = type.GetMethod("OnStart", Flags);

                foreach (var service in ServicesToRun)
                {
                    method.Invoke(service, new object[] { args });
                }

                Console.WriteLine("Press any key to exit!");
                Console.ReadLine();

                foreach (var service in ServicesToRun)
                {
                    service.Stop();
                }
            }
            else
            {
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
