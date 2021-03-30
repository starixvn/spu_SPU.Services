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
    using Newtonsoft.Json;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    class Program
    {
        static DataCenterService dataService = new DataCenterService();
        static string messsageLog = string.Empty;

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
                    messsageLog = "Probation monitor is starting...";
                    customers = dataService.GetProbationCustForScanning(batchSize);

                    if(customers == null || !customers.Any())
                    {
                        return;
                    }

                    messsageLog += "\nProbation monitor - Get custs: " + customers.Count();
                    ClassifyCustomer(customers);
                    dataService.UpdateLastScanDateForProbationCust(customers);
                    messsageLog += "\nProbation monitor - Update scan date done - custId: " + JsonConvert.SerializeObject(customers);
                    messsageLog += "\nProbation monitor - DONE FOR 1 BATCH - " + customers.Count();
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);
                }
                finally
                {
                    LogService.Info(messsageLog);
                    messsageLog = string.Empty;
                }
            } while (customers == null || customers.Any());
        }

        static void ClassifyCustomer(IEnumerable<ProbationModel> customers)
        {
            messsageLog += "\nProbation monitor - ClassifyCustomer: " + customers.Count();
            var custs = dataService.ClassifyCategoryForProbationCust(customers);
            var failedCusts = custs.Where(cus => !cus.IsPassed);
            var passedCusts = custs.Where(cus => cus.IsPassed);
            messsageLog += "\nProbation monitor - ClassifyCustomer - Pass cust: " + JsonConvert.SerializeObject(passedCusts);
            messsageLog += "\nProbation monitor - ClassifyCustomer - Failed cust: " + failedCusts.Count();

            if (failedCusts.Any())
            {
                dataService.InsertProbationNotification(customers, failedCusts);
                messsageLog += "\nProbation monitor - done";
            }
        }
    }
}
