namespace Fanex.NAP.CTS.RobotUsers.Console
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

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

            LogService.Info("Robot User - Start running...");
            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);
            var creditSites = ConfigurationManager.AppSettings["Credit-Sites"].ToString();
            var executedTime = DateTime.Now.ToString(Constants.CONVERT_TO_SQL_DATETIME);
            var paramId = 3;

            try
            {
                var previousTimingExecution = dataService.GetPreviousTimingExecution(paramId)?.SingleOrDefault();
                LogService.Info("Robot User - Last running: " + previousTimingExecution.ToString());

                var fromDate = DateTime.Parse(previousTimingExecution.ParameterValue);
                var robotUsers = dataService.GetRobotUser(fromDate, creditSites);
                LogService.Info("Robot User - GetRobotUser: " + robotUsers.Count());
                if (robotUsers.Any())
                {
                    var robotUsersChunks = robotUsers.Chunk(batchSize).ToList();
                    foreach (var robotUserList in robotUsersChunks)
                    {
                        var custClasses = dataService.InsertRobotUsers(string.Join(",", robotUserList.Select(c => c.CustId)));
                        LogService.Info("Robot User - InsertRobotUsers done: " + custClasses.Count());
                        dataService.UpdateCustomerGeneralClassificationtoMainDB(custClasses);
                        LogService.Info("Robot User - Update cust class on MainDB done!");
                        Thread.Sleep(300);
                    }
                }

                dataService.UpdateTimingExecution(executedTime, paramId);
                LogService.Info("Robot User - UpdateTimingExecution: " + executedTime.ToString());
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
                Console.WriteLine(logMessage);
            }
        }
    }
}
