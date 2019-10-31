namespace Fanex.NAP.FPS.Renovation.DataTransform.Device.Service
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.FPS.Renovation.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceProcess;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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
