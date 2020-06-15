namespace Fanex.NAP.CTS.ClassifyCustomer.Daily
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
                LogService.Info("Start to Classify Daily");
                var customers = dataService.GetCustomerForRunDaily(3,1000);
                if (customers.Any())
                {
                    var custGroupByBatch = customers.GroupBy(c => c.BatchID);
                    foreach (var custGroup in custGroupByBatch)
                    {
                        var task = Task.Run(() =>
                        {                            
                            var newListCustCategory = dataService.ClassifyCustomerDaily(customers);
                            LogService.Info("ClassifyCustomerDaily: newListCustCategory" + newListCustCategory.Count());
                            string jsonData = JsonConvert.SerializeObject(newListCustCategory);

                            dataService.AutoTagClassification(jsonData);
                        });

                        Task.Delay(10);
                    }

                    Task.WaitAll();
                    LogService.Info("ClassifyCustomerDaily done");
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
