using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Services;
using Fanex.NAP.SPU.Core;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Fanex.NAP.CTS.AutoTagProblemAccount.Console
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
                var previousTimingExecution = dataService.GetPreviousTimingExecution()?.SingleOrDefault();

                var problemAccounts = new List<ProblemAccountsModel>();
                var savedDataTime = DateTime.Now;

                if (string.IsNullOrEmpty(previousTimingExecution.ParameterValue))
                {
                    problemAccounts = dataService.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, savedDataTime).ToList();
                    problemAccounts.AddRange(dataService.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, savedDataTime));
                }
                else
                {
                    problemAccounts = dataService.GetProblemAccounts(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)).ToList();
                    problemAccounts.AddRange(dataService.GetProblemAccounts_CN88(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)));
                }

                LogService.Info("AutoTag Problem Account - problemAccounts: " + problemAccounts.Count);
                dataService.UpdateTimingExecution(savedDataTime.ToString(Constants.CONVERT_TO_SQL_DATETIME));

                if (problemAccounts.Any())
                {
                    var result = problemAccounts.Select(a => new InsertProblemAccountsModel
                    {
                        CustId = a.CustID,
                        CategoryId = a.ReasonID
                    });

                    var problemAccount4MainDB = problemAccounts.Select(a => new ProblemAccountsModel
                    {
                        CustID = a.CustID,
                        ReasonID = a.ReasonID
                    }).Where(c => c.CategoryIdMapping != -1).ToList();
                    LogService.Info("AutoTag Problem Account - problemAccount4MainDB: " + problemAccount4MainDB.Count);

                    var jsonData = JsonConvert.SerializeObject(result);

                    dataService.InsertProblemAccounts(jsonData);

                    var specialCustomerClass = dataService.GetAllSpecialCustomerClass();
                    problemAccount4MainDB.RemoveAll(item => specialCustomerClass.Contains(item.CustID));

                    if (problemAccount4MainDB != null && problemAccount4MainDB.Any())
                    {
                        var countRetry = 0;

                        while (countRetry < AppSettings.RetryTime)
                        {
                            try
                            {
                                Logger.Log.Info("AutoTag Problem Account - Insert ProblemAccount Classification To MainDB started with " + problemAccount4MainDB.Count());
                                dataService.InsertProblemAccountClassificationToMainDB(problemAccount4MainDB);
                                countRetry = AppSettings.RetryTime + 1;
                                Logger.Log.Info("AutoTag Problem Account - Insert ProblemAccount Classification To MainDB finished");
                            }
                            catch (Exception ex)
                            {
                                var logMessage = new LogMessage(ex.Message)
                                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                                LogService.Error($"AutoTag Problem Account - Insert ProblemAccount Classification To MainDB service - Start Exception {logMessage}");
                                countRetry += 1;
                            }
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
    }
}
