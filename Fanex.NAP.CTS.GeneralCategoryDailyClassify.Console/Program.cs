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

namespace Fanex.NAP.CTS.GeneralCategoryDailyClassify.Console
{
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

            DailyClassify();

        }

        private static void DailyClassify()
        {
            IEnumerable<CustomerClassificationModel> customers = null;            
            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);
            int countTotalCusts = 0;

            var dataCenterService = new DataCenterService();

            do
            {
                try
                {
                    customers = dataCenterService.GetCustomerForRunDaily(1, batchSize);

                    if (customers != null && customers.Any())
                    {
                        var custGroupByBatch = customers.GroupBy(c => c.BatchID);
                        foreach (var custGroup in custGroupByBatch)
                        {
                            dataCenterService.ClassifyCustomerGeneralCategoryDaily(string.Join(",", custGroup.Select(c => c.CustId)), -1, true);
                            string jsonData = JsonConvert.SerializeObject(custGroup.Select(a => new { a.CustId, a.SportGroupId }));
                            dataCenterService.ClassifyCustomerDailyDone(jsonData);
                            countTotalCusts += custGroup.Count();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);
                }

            } while (customers == null || customers.Any());

            LogService.Info("Classify Normal customer daily: " + countTotalCusts + " Date: " + DateTime.Now.ToString());
        }
    }
}
