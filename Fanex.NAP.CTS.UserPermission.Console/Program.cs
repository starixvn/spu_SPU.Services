namespace Fanex.NAP.CTS.UserPermission.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.DCS.Model;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var permissionService = new PermissionService();

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
                //permissionService
                //    .Start()
                //    .SyncUserPermission(FSFunctionKeys.AssociatedAccountMonitor, FSFunctionKeys.AssociatedAccountMonitorKey)
                //    .SyncUserPermission(FSFunctionKeys.ProbationManagement, FSFunctionKeys.ProbationManagementKey)
                //    .InsertUserPermission();

                var data = new List<RawTransactionModel>();
                var _dataTransformService = new DataTransformService();

                do
                {
                    data = _dataTransformService.GetECRawTrans(10000);

                    if (data != null && data.Any())
                    {
                        _dataTransformService.InsertECRawTrans(JsonConvert.SerializeObject(data));
                        LogService.Info($"Insert EC RawTrans - {JsonConvert.SerializeObject(data)} - {DateTime.Now}");
                        //Console.WriteLine(JsonConvert.SerializeObject(data));
                    }
                } while (data != null && data.Any());
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
