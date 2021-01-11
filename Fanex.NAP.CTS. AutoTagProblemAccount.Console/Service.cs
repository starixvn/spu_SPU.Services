namespace Fanex.NAP.CTS.AutoTagProblemAccount.Console
{
   
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Service
    {
        private static IDataCenterService _dataCenterService;
        private static IPushDataToExternalService _pushDataToExternalService;

        public Service()
        {
            _dataCenterService = new DataCenterService();
            _pushDataToExternalService = new PushDataToExternalService();
        }

        public void Process()
        {
            var logData = new List<MultipleProblemAccountsModel>();

            try
            {
                var result = GetProblemAccounts();

                var isWriteLogInfo = AppSettings.IsWriteLogInfo;

                if (isWriteLogInfo)
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
                                PushDataToExternal(custIds, isWriteLogInfo);
                            }
                        }
                    }
                }

                _dataCenterService.UpdateTimingExecution(result.BackupTimingExecution.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.PARAMETER_VALUE);

                if (isWriteLogInfo)
                    LogService.Info($"AutoTagProblemAccount.Service - Done!");
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage("AutoTagProblemAccount.Service - " + ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                if (logData != null && logData.Any())
                    logMessage += $"Full data input: {JsonConvert.SerializeObject(logData)}";

                LogService.Error(logMessage);
            }
        }


        private static GetMultiProblemAccountsModel GetProblemAccounts()
        {
            var previousTimingExecution = _dataCenterService.GetPreviousTimingExecution(Constants.PARAMETER_VALUE)?.SingleOrDefault();

            var problemAccounts = new List<MultipleProblemAccountsModel>();
            var savedDataTime = DateTime.Now;

            if (string.IsNullOrEmpty(previousTimingExecution.ParameterValue))
            {
                problemAccounts = _dataCenterService.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, null).ToList();
                problemAccounts.AddRange(_dataCenterService.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, null));
            }
            else
            {
                if (AppSettings.IsFirstTimeRunForIBC)
                    problemAccounts = _dataCenterService.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, null).ToList();
                else
                    problemAccounts = _dataCenterService.GetProblemAccounts(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)).ToList();


                if (AppSettings.IsFirstTimeRunForCN88)
                    problemAccounts.AddRange(_dataCenterService.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, null));
                else
                    problemAccounts.AddRange(_dataCenterService.GetProblemAccounts_CN88(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)));
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

            return _dataCenterService.InsertProblemAccounts(inputData, false).CustIds;
        }

        private static void PushDataToExternal(IEnumerable<int> custIds, bool isWriteLogInfo)
        {
            var lastestClassifications = _dataCenterService.GetLatestClassification(string.Join(",", custIds));

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                bool isSyncMainSuccessfully = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, AppSettings.RetryTime);

                if (isSyncMainSuccessfully)
                {
                    _dataCenterService.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));

                    if (isWriteLogInfo)
                        LogService.Info($"AutoTagProblemAccount.Service - Sync CC done with :{JsonConvert.SerializeObject(lastestClassifications)}");
                }

                _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);

                if (isWriteLogInfo)
                    LogService.Info($"AutoTagProblemAccount.Service - Push Soon88 done with :{JsonConvert.SerializeObject(lastestClassifications)}");
            }
        }
    }
}