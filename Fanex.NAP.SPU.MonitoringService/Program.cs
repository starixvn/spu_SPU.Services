namespace Fanex.NAP.SPU.MonitoringService
{
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.FS.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            DbSettingProviderManager
                 .StartNewSession()
                 .WithSqlServer()
                 .UseDefaultDbSettingProvider("~/App_Data/SPNames.xml")
                 .UseDefaultConnectionStringProvider()
                 .Run();

            LogManager.SetDefaultLogCategory(AppSetting.FanexLogCategory)
                .Use(new List<ILogEngine>
                {
                        new SentryLogging(new SentryEngineOptions
                        {
                            Dsn = new Dsn(AppSetting.SentryLogging),
                        WriteLogTimeoutSeconds = 30
                        })
                });

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Runner()
            };

            if (System.Environment.UserInteractive)
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
