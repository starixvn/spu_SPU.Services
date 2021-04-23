namespace Fanex.NAP.DCS.ArchiveUnusedDeviceCode.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.SPU.Core;
    using Sentry;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            var service = new Service();

            DbSettingProviderManager
                 .StartNewSession()
                 .WithSqlServer()
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

            service.Process();
        }
    }
}
