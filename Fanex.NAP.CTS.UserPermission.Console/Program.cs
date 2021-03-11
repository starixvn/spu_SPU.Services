namespace Fanex.NAP.CTS.UserPermission.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.SPU.Core;
    using Sentry;
    using System;
    using System.Collections.Generic;

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
                permissionService
                    .SyncUserPermission(FSFunctionKeys.AssociatedAccountMonitor, FSFunctionKeys.AssociatedAccountMonitorKey)
                    .SyncUserPermission(FSFunctionKeys.ProbationManagement, FSFunctionKeys.ProbationManagementKey);
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
