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
    using Newtonsoft.Json;
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
            var pushDataToExternalService = new PushDataToExternalService();

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

            var executedTime = DateTime.Now.ToString(Constants.CONVERT_TO_SQL_DATETIME);

            try
            {
                var previousTimingExecution = dataService.GetPreviousTimingExecution(Constants.PARAMETER_ROBOTUSER_ID)?.SingleOrDefault();

                var fromDate = DateTime.Parse(previousTimingExecution.ParameterValue);
                var robotUsers = dataService.GetRobotUser(fromDate, AppSettings.CreditSizes);
                if (robotUsers.Any())
                {
                    var robotUsersChunks = robotUsers.Chunk(AppSettings.BatchSize).ToList();
                    foreach (var robotUserList in robotUsersChunks)
                    {
                        var output = dataService.InsertRobotUsers(string.Join(",", robotUserList.Select(c => c.CustId)));

                        if (string.IsNullOrEmpty(output.ErrorMessage) && output.CustClassification != null && output.CustClassification.Any())
                        {
                            bool isSyncMainSuccessfully = pushDataToExternalService.PushCustomerClassificationToMain(output.CustClassification, AppSettings.RetryTime);

                            if (isSyncMainSuccessfully)
                            {
                                dataService.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(output.CustClassification));
                            }
                        }
                        else if(!string.IsNullOrEmpty(output.ErrorMessage))
                        {
                            LogService.Error($"Service Robot User exception: {output.ErrorMessage} - Data: {JsonConvert.SerializeObject(robotUserList.Select(c => c.CustId))}");
                        }

                        Thread.Sleep(300);
                    }
                }

                dataService.UpdateTimingExecution(executedTime, Constants.PARAMETER_ROBOTUSER_ID);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage("Robot User -" + ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }
    }
}
