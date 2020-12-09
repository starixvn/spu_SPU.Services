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


            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);
            var backupData = new List<MultipleProblemAccountsModel>();
            try
            {
                var result = GetProblemAccounts(dataService);

                if (result.ProblemAccounts != null && result.ProblemAccounts.Any())
                {
                    backupData = result.ProblemAccounts;

                    var lastestCustIds = InsertProblemAccountToCTS(result.ProblemAccounts, dataService);

                    dataService.UpdateTimingExecution(result.BackupTimingExecution.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.PARAMETER_VALUE);

                    if (lastestCustIds != null && lastestCustIds.Any())
                    {
                        var lastestCustIdsChunk  = lastestCustIds.Chunk(batchSize).ToList();

                        for (int i = 0; i < lastestCustIdsChunk.Count; i++)
                        {
                            var lastestClassifications = GetLatestClassificationForCusts(string.Join(",", lastestCustIdsChunk[i].Select(c => c)), dataService);

                            if (lastestClassifications != null && lastestClassifications.Any())
                            {
                                InsertProblemCustomerToMainDB(lastestClassifications.ToList(), dataService);
                            }
                        }
                    }

                    return;
                }

                dataService.UpdateTimingExecution(result.BackupTimingExecution.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.PARAMETER_VALUE);             
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                if(backupData != null && backupData.Any())
                    logMessage += $"Full data input: {JsonConvert.SerializeObject(backupData)}"; 

                LogService.Error(logMessage);
            }

            Console.ReadLine();
        }

        static GetMultiProblemAccountsModel GetProblemAccounts(DataCenterService dataService)
        {
            var previousTimingExecution = dataService.GetPreviousTimingExecution(Constants.PARAMETER_VALUE)?.SingleOrDefault();

            var problemAccounts = new List<MultipleProblemAccountsModel>();
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

            return new GetMultiProblemAccountsModel()
            {
                ProblemAccounts = problemAccounts,
                BackupTimingExecution = savedDataTime
            };
        }

        static IEnumerable<string> InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts, DataCenterService dataService)
        {
            var result = problemAccounts.Select(a => new InsertProblemAccountsModel
            {
                CustId = a.CustID,
                CategoryId = a.ReasonID
            });

            var jsonData = JsonConvert.SerializeObject(result);
            
            return dataService.InsertProblemAccounts(jsonData);
        }

        static void InsertProblemAccountToMainDB(List<ProblemAccountForMainDBModel> problemAccounts, DataCenterService dataService)
        {
            var specialCustomerClass = dataService.GetAllSpecialCustomerClass();
            problemAccounts.RemoveAll(item => specialCustomerClass.Contains(item.CustID));

            if (problemAccounts != null && problemAccounts.Any())
            {
                var countRetry = 0;

                while (countRetry < AppSettings.RetryTime)
                {
                    try
                    {
                        dataService.InsertProblemAccountClassificationToMainDB(problemAccounts);
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

        static IEnumerable<ProblemAccountForMainDBModel> GetLatestCategoryForCusts(string custIds, DataCenterService dataService)
        {
            var lastestCategories = dataService.GetLastestCategory(custIds, true, false)?.Where(c => c.CategoryIdMapping != -99)?.Distinct();

            return lastestCategories;
        }

        static IEnumerable<ProblemCustomerForMainDBModel> GetLatestClassificationForCusts(string custIds, DataCenterService dataService)
        {
            var lastestClassification = dataService.GetLatestClassification(custIds, true)?.Distinct();

            return lastestClassification;
        }

        static void InsertProblemCustomerToMainDB(List<ProblemCustomerForMainDBModel> problemCustomers, DataCenterService dataService)
        {
            var specialCustomerClass = dataService.GetAllSpecialCustomerClass();
            problemCustomers.RemoveAll(item => specialCustomerClass.Contains(item.CustID));

            if (problemCustomers != null && problemCustomers.Any())
            {
                var countRetry = 0;

                while (countRetry < AppSettings.RetryTime)
                {
                    try
                    {
                        dataService.InsertProblemCustomerClassificationToMainDB(problemCustomers);
                        countRetry = AppSettings.RetryTime + 1;
                    }
                    catch (Exception ex)
                    {
                        var logMessage = new LogMessage(ex.Message)
                         .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                         .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                        LogService.Error($"AutoTag Problem Customer - Insert Problem Customer Classification To MainDB service - Start Exception {logMessage}");
                        countRetry += 1;
                    }
                }
            }
        }
    }
}
