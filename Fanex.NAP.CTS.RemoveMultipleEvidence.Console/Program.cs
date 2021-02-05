namespace Fanex.NAP.CTS.RemoveEvidenceMultipleAccounts.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.CTS.Model.RemoveEvidenceMultipleAccounts;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {

        private static ICustomerEvidenceService customerEvidenceService = new CustomerEvidenceService();
        private static ICustomerClassifyService customerClassifyService = new CustomerClassifyService();
        private const int clearActionId = 3;

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
                var customerEvidenceFromQueue = GetCustomerEvidenceFromQueue();

                if (customerEvidenceFromQueue != null && customerEvidenceFromQueue.Any())
                {
                    var customerEvidenceSerilizeJson = JsonConvert.SerializeObject(customerEvidenceFromQueue);

                    var result = customerEvidenceService.ModifyCustomerEvidence(customerEvidenceSerilizeJson, clearActionId);

                    if (result.ErrorCode == -1)
                    {
                        LogService.Error(result.ErrorMsg);
                    }
                    else
                    {
                        var processedIdList = customerEvidenceFromQueue.Select(x => x.Id);

                        var updateResult = customerEvidenceService.UpdateCustomerEvidence(string.Join(",", processedIdList));

                        if (updateResult.ErrorCode == -1)
                        {
                            LogService.Error(updateResult.ErrorMsg);
                        }
                        else
                        {
                            var custIds = customerEvidenceFromQueue.Select(x => (int)x.CustId).ToList();

                            customerClassifyService.RescanCustomerClassify(custIds, true, true, Constants.FLOWTYPERESCANCC.NEWMEMBER);
                        }
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
        }

        private static List<CustomerEvidenceQueueModel> GetCustomerEvidenceFromQueue()
        {
            return customerEvidenceService.GetCustomerEvidenceQueue().ToList();
        }
    }
}
