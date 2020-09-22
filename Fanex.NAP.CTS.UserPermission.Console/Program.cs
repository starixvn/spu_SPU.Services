namespace Fanex.NAP.CTS.UserPermission.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.Core.Models.ValueObject;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using Sentry;

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
                var permissionUserIds = GetPermissionUserIDs();

                if (permissionUserIds != null && permissionUserIds.Count > 0)
                {
                    dataService.InsertUserPermission(JsonConvert.SerializeObject(permissionUserIds));
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

        private static List<UserPermissionModel> GetPermissionUserIDs()
        {
            var associatedPermissionUserIds = new UserService().GetUserPolicies(FSFunctionKeys.AssociatedAccountMonitor, Int32.Parse(AppSettings.SiteId)) as List<UserPolicy>;
            var probationPermissionUserIds = new UserService().GetUserPolicies(FSFunctionKeys.ProbationManagement, Int32.Parse(AppSettings.SiteId)) as List<UserPolicy>;

            var permissionUserIds = new List<UserPermissionModel>();
            permissionUserIds.AddRange(PreparePermissionUserIds(associatedPermissionUserIds, 1));
            permissionUserIds.AddRange(PreparePermissionUserIds(probationPermissionUserIds, 2));

            return permissionUserIds;
        }

        private static List<UserPermissionModel> PreparePermissionUserIds(List<UserPolicy> permissionUserIds, int funcKey)
        {
            var output = new List<UserPermissionModel>();

            if (permissionUserIds != null && permissionUserIds.Count > 0)
            {
                permissionUserIds.ForEach(c => output.Add(new UserPermissionModel() { UserID = c.User.Id, FuncID = funcKey}));
            }

            return output;
        }
    }
}
