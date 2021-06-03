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

            var toDate = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");

            var service = new Service(directorySaveImage);

            service.MoveFileToArcFolder();

            var leagueId8Percents = service.GetLeagueIds(Consts.Endpoint_AutoTrading_LeagueIds, true);         
            var leagueId8PercentList = leagueId8Percents != string.Empty ? leagueId8Percents.Split(',').Select(int.Parse).ToList() : new List<int>();

            if (leagueId8PercentList.Any())
            {
                service.PostGenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Percent8, Consts.FileName_AutoTrading_Percent8, leagueId8PercentList);

                service.PostGenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Percent8LastWeek, Consts.FileName_AutoTrading_Percent8LastWeek, leagueId8PercentList);
            }
            else
            {
                LogService.Info($"List League ID 8% is empty! - {toDate}");
            }

            var leagueIdOver8Percents = service.GetLeagueIds(Consts.Endpoint_AutoTrading_LeagueIds, false);
            var leagueIdOver8PercentList = leagueIdOver8Percents != null ? leagueIdOver8Percents.Split(',').Select(int.Parse).ToList() : new List<int>();

            if (leagueIdOver8PercentList.Any())
            {
                service.PostGenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Over8Percent, Consts.FileName_AutoTrading_Over8Percent, leagueIdOver8PercentList);

                service.PostGenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_Over8PercentLastWeek, Consts.FileName_AutoTrading_OverPercent8LastWeek, leagueIdOver8PercentList);
            }
            else
            {
                LogService.Info($"List League ID over 8% is empty! - {toDate}");
            }

            service.PostGenerateAutoTradingSpreadToFile(Consts.Endpoint_AutoTrading_NewTA, Consts.FileName_AutoTrading_NewTA, new List<int>());

            if (service.IsSuccessAPIResponse)
            {
                //if (service.SendFileMessageViaLestalk())
                //{
                //    LogService.Info($"AutoTrading Spread SendFileMessageViaLestalk has sucessfully - {toDate}");
                //}         

                LogService.Info($"AutoTrading Spread has sucessfully - {toDate}");       
            }
            else
            {
                LogService.Info($"Generate data has failed - {toDate}");
            }
        }
    }
}
