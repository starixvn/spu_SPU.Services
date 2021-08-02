namespace Fanex.NAP.CTS.GeneralCategoryRealtimeClassify.Console
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
                //var categoryIds = "201, 202, 205, 231, 232, 233, 222, 221, 223, 212, 211, 213";  'New member' & 'Main Normal Account' & 'Main VIP'  & special categories

                var custsHasChanges = dataCenterService.GetCustomerHasChangeForRunRealtime();
        
                if (custsHasChanges.Any())
                {
                    var custChunks = custsHasChanges.Chunk(batchSize).ToList();

                    for (int i = 0; i < custChunks.Count; i++)
                    {
                        var custSportJson = JsonConvert.SerializeObject(custChunks[i]);

                        var custCategories = dataCenterService.GetCustomerClassificationForRunRealtime(custSportJson);

                        var classifyRealtime = custChunks[i].GroupJoin(custCategories,
                                                    custsHaschanges => custsHaschanges.CustId,
                                                    custsCategories => custsCategories.CustId,
                                                    (custsHaschanges, custsCategories) => new { custsHaschanges, custsCategories })
                                                    .SelectMany(
                                                        group => group.custsCategories.DefaultIfEmpty(),
                                                        (group, temp) => new CustomerClassificationModel
                                                        {
                                                            CustId = group.custsHaschanges.CustId,
                                                            SportGroupId = group.custsHaschanges.SportGroupId,
                                                            CategoryId = (temp != null) ? temp.CategoryId : 0
                                                        });

                        dataCenterService.ClassifyCustomerGeneralCategoryRealtime(classifyRealtime);

                        Thread.Sleep(3000);
                    }
                }

                dataCenterService.RealtimeUpdateLastScanTime(true);
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
