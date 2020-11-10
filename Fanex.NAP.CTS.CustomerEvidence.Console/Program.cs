namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
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
                            Dsn = new Dsn(AppSettings.SentryLogging),
                            WriteLogTimeoutSeconds = 30
                        })
                    });

            var service = new Service();
            service.ProcessCustEvidenceFromFile();
        }
    }
}
