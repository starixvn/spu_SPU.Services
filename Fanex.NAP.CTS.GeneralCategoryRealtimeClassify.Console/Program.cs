namespace Fanex.NAP.CTS.GeneralCategoryRealtimeClassify.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
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

            RealtimeClassify();

        }

        private static void RealtimeClassify()
        {
            try
            {
                var dataCenterService = new DataCenterService();
                var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);
                
                var customerHasChanges = dataCenterService.GetCustomerGeneralCategoryHasChangeForRunRealtime();
                Console.WriteLine("customerHasChanges: " + customerHasChanges.Count());

                if (customerHasChanges.Any())
                {
                    var custChunks = customerHasChanges.Chunk(batchSize).ToList();
                    for (int i = 0; i < custChunks.Count; i++)
                    {
                        var custSportJson = JsonConvert.SerializeObject(custChunks[i]);
                        var customers = dataCenterService.GetCustomerGeneralCategoryForRunRealtime(custSportJson);
                        Console.WriteLine("GetCustomerGeneralCategoryForRunRealtime: " + customers.Count());
                        dataCenterService.ClassifyCustomerGeneralCategoryRealtime(customers);
                        Thread.Sleep(3000);
                    }
                }

                dataCenterService.RealtimeUpdateLastScanTime(true);
                Console.WriteLine("RealtimeUpdateLastScanTime done");
                Console.ReadLine();
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
