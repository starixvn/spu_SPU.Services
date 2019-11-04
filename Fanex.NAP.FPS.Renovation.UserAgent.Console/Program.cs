namespace Fanex.NAP.FPS.Renovation.UserAgent.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.FPS.Renovation.Core;
    using Newtonsoft.Json;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    static class Program
    {
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

            var userAgent = new Services.Implement.UserAgent();

            try
            {
                var userAgents = userAgent.GetUserAgent(Core.AppSettings.SizeOfUserAgent);

                if (userAgents != null && userAgents.Any())
                {
                    var transformUserAgents = userAgent.TransformUserAgent(userAgents);

                    if (transformUserAgents.Any())
                    {
                        var userAgentJson = JsonConvert.SerializeObject(transformUserAgents);

                        userAgent.UpdateUserAgent(userAgentJson);
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }
    }
}
