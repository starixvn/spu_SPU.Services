namespace Fanex.NAP.CTS.ClassifyCustomer.Realtime.Console
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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NAP.FS.Core;

    class Program
    {
        static void Main(string[] args)
        {
            var dataService = new DataCenterService();
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

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                LogService.Info("Job Realtime is running");
                var customerHasChanges = dataService.GetCustomerHasChangeForRunRealtime();
                var custIDHasChanges = string.Join(",", customerHasChanges.Select(c => c.CustId));

                var customers = dataService.GetCustomerForRunRealtime(custIDHasChanges);
                LogService.Info("Customers: " + customers.Count());

                if (customers.Any())
                {
                    var custChunks = customers.Chunk(1000);
                    foreach (var custs in custChunks)
                    {
                        LogService.Info("Start to Classify Realtime");
                        var newListCustCategory = dataService.ClassifyCustomerRealtime(custs);
                        LogService.Info("ClassifyCustomerRealtime: newListCustCategory" + newListCustCategory.Count());
                        string jsonData = JsonConvert.SerializeObject(newListCustCategory);
                        LogService.Info("ClassifyCustomerRealtime: jsonData" + jsonData);
                        dataService.AutoTagClassification(jsonData);
                        LogService.Info("ClassifyCustomerRealtime: AutoTagClassification done");
                    }
                }

                watch.Stop();
                LogService.Info("Job Realtime run completely! Elapsed time: " + watch.ElapsedMilliseconds + "(ms)");
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
