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
    using Fanex.NAP.FS.Core;
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

            var logData = new List<MultipleProblemAccountsModel>();
            var pushDataToExternalService = new PushDataToExternalService();

            try
            {
                var result = GetProblemAccounts();

                if (result.ProblemAccounts != null && result.ProblemAccounts.Any())
                {
                    logData = result.ProblemAccounts;

                    var output = InsertProblemAccountToCTS(result.ProblemAccounts);

                    if (string.IsNullOrEmpty(output.ErrorMessage) & output.CustIds != null && output.CustIds.Any())
                    {
                        var custIdsChunk  = output.CustIds.Chunk(AppSettings.BatchSize);

                        foreach(var custs in custIdsChunk)
                        {
                            var lastestClassifications = WrapperDataCenterService.Instance.GetLatestClassification(string.Join(",", custs));

                            if (lastestClassifications != null && lastestClassifications.Any())
                            {
                                bool isSyncMainSuccessfully = pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, AppSettings.RetryTime);

                                if (isSyncMainSuccessfully)
                                {
                                    WrapperDataCenterService.Instance.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));
                                }

                                pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                            }
                        }
                    }
                }

                WrapperDataCenterService.Instance.UpdateTimingExecution(result.BackupTimingExecution.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.PARAMETER_VALUE);             
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                if(logData != null && logData.Any())
                    logMessage += $"Full data input: {JsonConvert.SerializeObject(logData)}"; 

                LogService.Error(logMessage);
            }
        }
        
        private static GetMultiProblemAccountsModel GetProblemAccounts()
        {
            var previousTimingExecution = WrapperDataCenterService.Instance.GetPreviousTimingExecution(Constants.PARAMETER_VALUE)?.SingleOrDefault();

            var problemAccounts = new List<MultipleProblemAccountsModel>();
            var savedDataTime = DateTime.Now;

            if (string.IsNullOrEmpty(previousTimingExecution.ParameterValue))
            {
                problemAccounts = WrapperDataCenterService.Instance.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, null).ToList();
                problemAccounts.AddRange(WrapperDataCenterService.Instance.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, null));
            }
            else
            {
                if (AppSettings.IsFirstTimeRunForIBC)
                    problemAccounts = WrapperDataCenterService.Instance.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, null).ToList();
                else
                    problemAccounts = WrapperDataCenterService.Instance.GetProblemAccounts(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)).ToList();


                if (AppSettings.IsFirstTimeRunForCN88)
                    problemAccounts.AddRange(WrapperDataCenterService.Instance.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, null));
                else
                    problemAccounts.AddRange(WrapperDataCenterService.Instance.GetProblemAccounts_CN88(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)));
            }

            return new GetMultiProblemAccountsModel()
            {
                ProblemAccounts = problemAccounts,
                BackupTimingExecution = savedDataTime
            };
        }

        private static InsertCategoryResultModel InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
            var result = problemAccounts.Select(a => new InsertProblemAccountsModel
            {
                CustId = a.CustID,
                CategoryId = a.ReasonID
            });

            var jsonData = JsonConvert.SerializeObject(result);
            
            return WrapperDataCenterService.Instance.InsertProblemAccounts(jsonData);
        }
    }
}
