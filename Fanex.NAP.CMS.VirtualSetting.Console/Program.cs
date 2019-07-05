namespace Fanex.NAP.CMS.VirtualSetting.Console
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
                    .SetDefaultLogCategory(AppSetting.FanexLogCategory)
                    .Use(new List<ILogEngine>
                    {
                        new SentryLogging(new SentryEngineOptions
                        {
                            Dsn = new Dsn(AppSetting.SentryLogging)
                        })
                    });

                var service = new Service();

                if (service.IsStopService())
                {
                    return;
                }

                if (service.IsExecutedDate())
                {
                    service
                        .StartTime()
                        .ExecuteForSoccer()
                        .ExecuteForSportbooks()
                        .CountAffectedMembers()
                        .SendMailExternal(AppSetting.MailingListId);
                }
                else
                {
                    service
                        .SendMailInternal(AppSetting.MailingIdToSPU);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VirtualSetting service - Start Exception {logMessage}");
            }
        }
    }
}