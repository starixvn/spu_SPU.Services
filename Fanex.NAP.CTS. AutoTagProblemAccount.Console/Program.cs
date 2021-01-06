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

                var isWriteLogInfo = AppSettings.IsWriteLogInfo;

                if(isWriteLogInfo)
                    LogService.Info($"AutoTagProblemAccount.Service - Get PA From NAPCM & CN88 with Data: {JsonConvert.SerializeObject(result)}");

                if (result.ProblemAccounts != null && result.ProblemAccounts.Any())
                {
                    logData = result.ProblemAccounts;

                    var problemAccountsChunk = result.ProblemAccounts.Chunk(AppSetting.UpdateInternalChunkSize);

                    foreach (var problemAccs in problemAccountsChunk)
                    {
                        var custs = InsertProblemAccountToCTS(problemAccs);

                        if (isWriteLogInfo)
                            LogService.Info($"AutoTagProblemAccount.Service - InsertProblemAccountToCTS done with: {JsonConvert.SerializeObject(problemAccs)}");

                        if (custs != null && custs.Any())
                        {
                            var custIdsChunk = custs.Chunk(AppSettings.PushCCToExternalChunkSize);

                            foreach (var custIds in custIdsChunk)
                            {
                                PushDataToExternal(custIds, isWriteLogInfo, pushDataToExternalService);
                            }
                        }
                    }
                }

                WrapperDataCenterService.Instance.UpdateTimingExecution(result.BackupTimingExecution.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.PARAMETER_VALUE);

                if (isWriteLogInfo)
                    LogService.Info($"AutoTagProblemAccount.Service - Done!");
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage("AutoTagProblemAccount.Service - " + ex.Message)
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
                ProblemAccounts = problemAccounts.Distinct().ToList(),
                BackupTimingExecution = savedDataTime
            };
        }

        private static IEnumerable<int> InsertProblemAccountToCTS(IEnumerable<MultipleProblemAccountsModel> problemAccounts)
        {
            var inputData = problemAccounts.Select(a => new InsertProblemAccountsModel
            {
                CustId = a.CustID,
                CategoryId = a.ReasonID
            }).ToList();
            
            return WrapperDataCenterService.Instance.InsertProblemAccounts(inputData).CustIds;
        }

        private static void PushDataToExternal (IEnumerable<int> custIds, bool isWriteLogInfo, PushDataToExternalService service)
        {
            var lastestClassifications = WrapperDataCenterService.Instance.GetLatestClassification(string.Join(",", custIds));

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                bool isSyncMainSuccessfully = service.PushCustomerClassificationToMain(lastestClassifications, AppSettings.RetryTime);

                if (isSyncMainSuccessfully)
                {
                    WrapperDataCenterService.Instance.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));

                    if (isWriteLogInfo)
                        LogService.Info($"AutoTagProblemAccount.Service - Sync CC done with :{JsonConvert.SerializeObject(lastestClassifications)}");
                }

                service.PushCustomerClassificationToSoon88(lastestClassifications);

                if (isWriteLogInfo)
                    LogService.Info($"AutoTagProblemAccount.Service - Push Soon88 done with :{JsonConvert.SerializeObject(lastestClassifications)}");
            }
        }
    }
}
