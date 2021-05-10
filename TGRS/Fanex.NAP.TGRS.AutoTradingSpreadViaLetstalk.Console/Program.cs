namespace Fanex.NAP.TGRS.AutoTradingSpreadViaLetstalk.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

            var leagueIds = service.GetLeagueIds(Consts.Endpoint_AutoTrading_LeagueIds, true);         
            var leagueIdList = leagueIds != string.Empty ? leagueIds.Split(',').Select(int.Parse).ToList() : new List<int>();

            if (leagueIdList.Any())
            {
                service.PostGenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Percent8, Consts.FileName_AutoTrading_Percent8, leagueIdList);

                service.PostGenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Percent8LastWeek, Consts.FileName_AutoTrading_Percent8LastWeek, leagueIdList);
            }

            var leagueIds2 = service.GetLeagueIds(Consts.Endpoint_AutoTrading_LeagueIds, false);
            var leagueIdList2 = leagueIds2 != null ? leagueIds2.Split(',').Select(int.Parse).ToList() : new List<int>();

            if (leagueIdList2.Any())
            {
                service.PostGenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Over8Percent, Consts.FileName_AutoTrading_Over8Percent, leagueIdList2);
            }            

        }
    }
}
