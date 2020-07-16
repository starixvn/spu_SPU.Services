using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.CTS.Services;
using Fanex.NAP.SPU.Core;
using Fanex.NAP.SPU.Model;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ArchiveTransaction.Console
{
    class Program
    {
        static DataCenterService dataCenterService = new DataCenterService();

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
                        Dsn = new Dsn(AppSettings.SentryLogging)
                    })
               });

            try
            {
                int batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["BatchSize"]);
                int noOfBatch = Convert.ToInt32(ConfigurationManager.AppSettings["NoOfBatch"]);
                int delayTime = Convert.ToInt32(ConfigurationManager.AppSettings["DelayTime"]);
                var trans = dataCenterService.DCSMoveTransToTransaction07GetPackage(batchSize, noOfBatch);
                if (trans.Any())
                {
                    ExecuteAsync(trans, delayTime).Wait();
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

        static async Task ExecuteAsync(IEnumerable<TransactionModel> trans, int delayTime)
        {
            var transByBatch = trans.GroupBy(t => t.BatchId);
            var tasks = new List<Task>();

            foreach (var tranGroup in transByBatch)
            {
                var task = Task.Run(() =>
                {
                    var transIdList = string.Join(",", tranGroup.Select(t => t.TransId));
                    dataCenterService.DCSMoveTransToTransaction07(transIdList);
                });

                await Task.Delay(delayTime);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
