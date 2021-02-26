namespace Fanex.NAP.CMS.VRMemberBigStake.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;

    static class Program
    {
        static void Main(string[] args)
        {
            try
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

                var service = new Service();

                service
                    .StartTime()
                    .InsertVRMemberBigStake()
                    .GetMemberBigStakePerformance()
                    .UpdateMemberBigStakeFirstBetDate()
                    .UpdateMemberBigStakeStatus()
                    .SendMail(AppSettings.MailingListId)
                    .EndTime();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VR - Update Member Big Stake service - Main Exception {logMessage}");
            }
        }
    }
}
