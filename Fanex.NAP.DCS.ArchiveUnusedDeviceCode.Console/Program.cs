namespace Fanex.NAP.DCS.ArchiveUnusedDeviceCode.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.SPU.Core;
    using Sentry;
    using System.Collections.Generic;

    class Program
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

            var service = new Service();
            service.Process();
        }
    }
}
