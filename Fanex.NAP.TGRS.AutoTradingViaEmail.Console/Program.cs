namespace Fanex.NAP.TGRS.AutoTradingViaEmail.Console
{    
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.IO;

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

            var directorySaveFile = AppDomain.CurrentDomain.BaseDirectory + @"Attachment\";
            var endpoint = "AutoTrading/GetAutoTradingEventLeaguesExcel";
            var fromDate = "07-Nov-2019";
            var toDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToString("dd-MMM-yyyy");
            var saveFileName = "Model TA Performance Event Details - " + fromDate + " to " + toDate + ".xls";            

            if (!Directory.Exists(directorySaveFile))
            {
                Directory.CreateDirectory(directorySaveFile);
            }
            
            var service = new Service(directorySaveFile)
                .MoveFileToArcFolder()
                .GenerateAutoTradingToFile(endpoint, saveFileName)
                .SendMailNotification(saveFileName);
        }

    }
}