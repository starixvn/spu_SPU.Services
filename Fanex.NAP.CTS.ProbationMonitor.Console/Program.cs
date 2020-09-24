namespace Fanex.NAP.CTS.ProbationMonitor.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    class Program
    {
        static DataCenterService dataService = new DataCenterService();

        static void Main(string[] args)
        {
            IEnumerable<ProbationModel> customers = null;
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

            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);

            do
            {
                try
                {
                    customers = dataService.GetProbationCustForScanning(500);
                    UpdateMargin(customers);
                    ClassifyCustomer(customers);
                    dataService.UpdateLastScanDateForProbationCust(customers);
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();
                    
                    LogService.Error(logMessage);
                }
            } while (customers == null || customers.Any());
        }

        static void UpdateMargin(IEnumerable<ProbationModel> customers)
        {
            var losers = dataService.CheckLoserForProbationCust(customers);
            if (losers.Any())
            {
                dataService.InsertProbationNotification(customers, losers, 1);
            }
        }

        static void ClassifyCustomer(IEnumerable<ProbationModel> customers)
        {
            var failedCusts = dataService.FinalizeForProbationCust(customers).Where(cus => !cus.IsPassed);
            if (failedCusts.Any())
            {
                dataService.InsertProbationNotification(customers, failedCusts, 2);
            }
        }
    }
}
