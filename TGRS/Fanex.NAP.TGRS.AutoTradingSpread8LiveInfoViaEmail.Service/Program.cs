namespace Fanex.NAP.TGRS.AutoTradingSpread8LiveInfoViaEmail.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.ServiceProcess;

    static class Program
    {
        static void Main(string[] args)
        {
            LogManager
                    .SetDefaultLogCategory(AppSettings.FanexLogCategory)
                    .Use(new List<ILogEngine>
                    {
                        new SentryLogging(new SentryEngineOptions
                        {
                            Dsn = new Dsn(AppSettings.SentryLogging),
                            WriteLogTimeoutSeconds = 30
                        })
                    });

            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new Runner()
            //};

            //if (System.Environment.UserInteractive)
            //{
            //    // run console
            //    var type = typeof(ServiceBase);
            //    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
            //    var method = type.GetMethod("OnStart", Flags);

            //    foreach (var service in ServicesToRun)
            //    {
            //        method.Invoke(service, new object[] { args });
            //    }

            //    Console.WriteLine("Press any key to exit!");
            //    Console.ReadLine();

            //    foreach (var service in ServicesToRun)
            //    {
            //        service.Stop();
            //    }
            //}
            //else
            //{
            //    ServiceBase.Run(ServicesToRun);
            //}

            Test();
        }

        static void Test()
        {
            var endpoint = "AutoTrading/GetAutoTradingSpread8LiveExcel";
            var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd-MMM-yyyy");
            var saveFileName = "Live Performance Report - " + toDate + ".xlsx";
            var directorySaveFile = AppDomain.CurrentDomain.BaseDirectory + @"Attachment\";

            if (!Directory.Exists(directorySaveFile))
            {
                Directory.CreateDirectory(directorySaveFile);
            }

            var service = new Service(directorySaveFile)
                    .MoveFileToArcFolder()
                    .GenerateAutoTradingSpread8LiveToFive(endpoint, saveFileName);
        }
    }
}
