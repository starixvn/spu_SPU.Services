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
    using System.Configuration;
    using System.Threading;

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

            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);

            try
            {
                var customerHasChanges = dataService.GetCustomerHasChangeForRunRealtime();

                if (customerHasChanges.Any())
                {
                    var custChunks = customerHasChanges.Chunk(batchSize).ToList();
                    for (int i = 0; i < custChunks.Count; i++)
                    {
                        var custSportJson = JsonConvert.SerializeObject(custChunks[i]);
                        var customers = dataService.GetCustomerForRunRealtime(custSportJson);
                        dataService.ClassifyCustomerRealtime(customers);
                        Thread.Sleep(30000);
                    }

                    dataService.RealtimeUpdateLastScanTime(false);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
                Console.WriteLine(logMessage);
            }
        }
    }
}
