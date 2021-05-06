namespace Fanex.NAP.TGRS.AutoTradingSpreadViaLetstalk.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    class Program
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

            service.MoveFileToArcFolder();

            service.GenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Percent8, Consts.FileName_AutoTrading_Percent8);

            service.GenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Percent8LastWeek, Consts.FileName_AutoTrading_Percent8LastWeek);

            //service.GenerateAutoTradingSpread8LiveToFile(Consts.Endpoint_AutoTrading_Percent8Virtual, Consts.FileName_AutoTrading_Percent8Virtual);

            //service.GenerateAutoTradingSpread8LiveToFile(Consts.Endpoint_AutoTrading_Percent8RealEx8RM, Consts.FileName_AutoTrading_Percent8RealEx8RM);
        }
    }
}
