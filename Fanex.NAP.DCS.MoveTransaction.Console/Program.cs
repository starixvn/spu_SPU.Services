namespace Fanex.NAP.DCS.MoveTransaction.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.SPU.Core;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    static class Program
    {
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
            var batchSize = AppSettings.SizeOfTransaction;
            var isCompleted = false;

            try
            {
                var dataCenterService = new DataCenterService();
                var dateForArchived = dataCenterService.GetScheduleForRunArchiveTrans07();
                foreach (var runningDate in dateForArchived)
                {
                    isCompleted = false;
                    LogService.Info("Start archive Transaction07 - move data: " + runningDate);
                    while (!isCompleted)
                    {
                        dataCenterService.MoveDataArchiveTransaction07(runningDate, batchSize, ref isCompleted);
                    }

                    isCompleted = false;
                    LogService.Info("Start archive Transaction07 - delete data: " + runningDate);
                    while (!isCompleted)
                    {
                        dataCenterService.DeleteDataArchiveTransaction07(runningDate, ref isCompleted);
                    }
                }

                LogService.Info("Archive Transaction07 completely! ");
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
