using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;
using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry;
using Fanex.NAP.Core.Models.ValueObject;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.CTS.Services;
using Fanex.NAP.SPU.Core;
using Sentry;

namespace Fanex.NAP.CTS.UserPermission.Console
{
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
                var userIds = GetPermissionUserIDs();

                var preUserData = Helper.ReadXML(AppSettings.IntermediateUserFileName);

                if (!CheckDataMatching(preUserData, userIds))
                {
                    dataService.InsertUserPermission(userIds, FSFunctionKeys.AssociatedAccountMonitor);
                    Helper.WriteXML(AppSettings.IntermediateUserFileName, userIds);
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

        /// <summary>
        /// Check whether there are any change of user AAM permission
        /// </summary>
        private static bool CheckDataMatching(string dataFromCache, string userIds)
        {
            if (string.IsNullOrEmpty(userIds) 
                || string.IsNullOrEmpty(dataFromCache)
                || !string.Equals(dataFromCache, userIds))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get users who have AAM permission
        /// </summary>
        private static string GetPermissionUserIDs()
        {
            var permissionUsers = new UserService().GetUserPolicies(FSFunctionKeys.AssociatedAccountMonitor, Int32.Parse(AppSettings.SiteId)) as List<UserPolicy>;

            if(permissionUsers != null && permissionUsers.Count > 0)
            {
                var userIDs = new StringBuilder();

                permissionUsers.Sort((a, b) => (a.User.Id.CompareTo(b.User.Id)));
                permissionUsers.ForEach(c => userIDs.Append(c.User.Id).Append(","));

                userIDs.Length--;

                return userIDs.ToString();
            }

            return null;
        }

    }
}
