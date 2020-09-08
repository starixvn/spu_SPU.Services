namespace Fanex.NAP.CMS.BasketballVR.Console
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
                    .UpdateVRForBasketball()
                    .UpdateVRForSpecialSportsbook()
                    .NotifyMessage();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VR Setting - Special Licensee service - Start Exception {logMessage}");
            }
        }
    }
}
