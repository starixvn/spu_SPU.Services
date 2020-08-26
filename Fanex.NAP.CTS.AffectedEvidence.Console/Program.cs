namespace Fanex.NAP.CTS.AffectedEvidence.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    public static class Program
    {
        static void Main(string[] args)
        {
            DbSettingProviderManager
                 .StartNewSession()
                 .WithMySql()
                 .WithSqlServer()
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

            try
            {
                var dataCenterService = new DataCenterService();
                var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);

                var custIds = dataCenterService.TransformAssociationAffectedEvidence(batchSize);

                // Set BetLimit
                LogService.Info("Transform Affected Evidence: custIds: " + string.Join(",", custIds));
                //dataCenterService.UpdateBetLimitByEvidenceRemoving(custIds);
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
