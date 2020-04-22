namespace Fanex.NAP.TGRS.AutoTrading.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

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

            var directorySaveImage = AppDomain.CurrentDomain.BaseDirectory + @"Attachment\";
            if (!Directory.Exists(directorySaveImage))
            {
                Directory.CreateDirectory(directorySaveImage);
            }

            var service = new Service(directorySaveImage);

            if (service.IsDateExecute())
            {
                service.MoveFileToArcFolder();
               
                var tasks = new List<Task>();
                tasks.Add(Task.Run(() => service.GenereateAutoTradingToFile(Consts.Endpoint_TAndAfterVRAdjustment, Consts.FileName_AutoTrading_PTAndAfterVRAdjustment)));
                tasks.Add(Task.Run(() => service.GenereateAutoTradingToFile(Consts.Endpoint_TAMonthly, Consts.FileName_AutoTrading_TAMonthly)));
                tasks.Add(Task.Run(() => service.GenereateAutoTradingToFile(Consts.Endpoint_RT3AndTAComparition, Consts.FileName_AutoTrading_RT3AndTAComparition)));

                Task.WhenAll(tasks.ToArray()).Wait();

                if (service.IsSuccessAPIResponse)
                {
                    if (service.SendFileMessageViaLestalk())
                    {
                        var toDate = DateTime.Today.ToString("dd/MM/yyyy hh:mm:ss");

                        LogService.Info($"SendFileMessageViaLestalk ({AppSettings.EnvironmentName}) - sucessfully - {toDate}");
                    }
                }
            }            
        }
    }
}