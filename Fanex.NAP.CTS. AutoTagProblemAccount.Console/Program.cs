namespace Fanex.NAP.CTS.AutoTagProblemAccount.Console
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
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

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
                var problemAccounts = GetProblemAccounts(dataService);

                if (problemAccounts != null && problemAccounts.Any())
                {
                    InsertProblemAccountToCTS(problemAccounts, dataService);
                    InsertProblemAccountToMainDB(problemAccounts, dataService);
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

        static List<ProblemAccountsModel> GetProblemAccounts(DataCenterService dataService)
        {
            var paramId = 2;
            var previousTimingExecution = dataService.GetPreviousTimingExecution(paramId)?.SingleOrDefault();

            var problemAccounts = new List<ProblemAccountsModel>();
            var savedDataTime = DateTime.Now;

            if (string.IsNullOrEmpty(previousTimingExecution.ParameterValue))
            {
                problemAccounts = dataService.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, savedDataTime).ToList();
                problemAccounts.AddRange(dataService.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, savedDataTime));
            }
            else
            {
                if (AppSettings.IsFirstTimeRunForIBC)
                    problemAccounts = dataService.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, savedDataTime).ToList();
                else
                    problemAccounts = dataService.GetProblemAccounts(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)).ToList();


                if (AppSettings.IsFirstTimeRunForCN88)
                    problemAccounts.AddRange(dataService.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, savedDataTime));
                else
                    problemAccounts.AddRange(dataService.GetProblemAccounts_CN88(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)));
            }

            dataService.UpdateTimingExecution(savedDataTime.ToString(Constants.CONVERT_TO_SQL_DATETIME), paramId);

            return problemAccounts;
        }

        static void InsertProblemAccountToCTS(List<ProblemAccountsModel> problemAccounts, DataCenterService dataService)
        {
            var result = problemAccounts.Select(a => new InsertProblemAccountsModel
            {
                CustId = a.CustID,
                CategoryId = a.ReasonID
            });

            var jsonData = JsonConvert.SerializeObject(result);
            
            dataService.InsertProblemAccounts(jsonData);
        }

        static void InsertProblemAccountToMainDB(List<ProblemAccountsModel> problemAccounts, DataCenterService dataService)
        {
            var problemAccount4MainDB = problemAccounts.Select(a => new ProblemAccountsModel
            {
                CustID = a.CustID,
                ReasonID = a.ReasonID
            }).Where(c => c.CategoryIdMapping != -99).ToList();
            
            var specialCustomerClass = dataService.GetAllSpecialCustomerClass();
            problemAccount4MainDB.RemoveAll(item => specialCustomerClass.Contains(item.CustID));

            if (problemAccount4MainDB != null && problemAccount4MainDB.Any())
            {
                var countRetry = 0;

                while (countRetry < AppSettings.RetryTime)
                {
                    try
                    {
                        dataService.InsertProblemAccountClassificationToMainDB(problemAccount4MainDB);
                        countRetry = AppSettings.RetryTime + 1;
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
}
