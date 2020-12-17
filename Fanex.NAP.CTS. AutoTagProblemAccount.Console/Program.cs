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


            var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);
            var backupData = new List<MultipleProblemAccountsModel>();
            try
            {
                var result = GetProblemAccounts();

                if (result.ProblemAccounts != null && result.ProblemAccounts.Any())
                {
                    backupData = result.ProblemAccounts;

                    var latestCustIds = InsertProblemAccountToCTS(result.ProblemAccounts);

                    WrapperDataCenterService.Instance.UpdateTimingExecution(result.BackupTimingExecution.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.PARAMETER_VALUE);

                    if (latestCustIds != null && latestCustIds.Any())
                    {
                        var latestCustIdsChunk  = latestCustIds.Chunk(batchSize);

                        foreach(var custids in latestCustIdsChunk)
                        {
                            var lastestClassifications = GetLatestClassificationForCusts(string.Join(",", custids));

                            if (lastestClassifications != null && lastestClassifications.Any())
                            {
                                var updatedProblemCustomers = InsertProblemCustomerToMainDB(lastestClassifications.ToList());

                                var resultAppliedCC = UpdateAppliedCustomerClassification(updatedProblemCustomers);
                                if (resultAppliedCC.ErrorCode != 0)
                                {
                                    LogService.Error(resultAppliedCC.ErrorMsg);
                                }
                            }
                        }
                    }

                    return;
                }

                WrapperDataCenterService.Instance.UpdateTimingExecution(result.BackupTimingExecution.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.PARAMETER_VALUE);             
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

            Console.WriteLine("Done for AutoTagProblemAccount");
            Console.ReadLine();
        }
        
        static GetMultiProblemAccountsModel GetProblemAccounts()
        {
            var previousTimingExecution = WrapperDataCenterService.Instance.GetPreviousTimingExecution(Constants.PARAMETER_VALUE)?.SingleOrDefault();

            var problemAccounts = new List<MultipleProblemAccountsModel>();
            var savedDataTime = DateTime.Now;

            if (string.IsNullOrEmpty(previousTimingExecution.ParameterValue))
            {
                problemAccounts = WrapperDataCenterService.Instance.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, savedDataTime).ToList();
                problemAccounts.AddRange(WrapperDataCenterService.Instance.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, savedDataTime));
            }
            else
            {
                if (AppSettings.IsFirstTimeRunForIBC)
                    problemAccounts = WrapperDataCenterService.Instance.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, savedDataTime).ToList();
                else
                    problemAccounts = WrapperDataCenterService.Instance.GetProblemAccounts(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)).ToList();


                if (AppSettings.IsFirstTimeRunForCN88)
                    problemAccounts.AddRange(WrapperDataCenterService.Instance.GetProblemAccounts_CN88(Constants.GET_ALL_INITIAL_DATA, savedDataTime));
                else
                    problemAccounts.AddRange(WrapperDataCenterService.Instance.GetProblemAccounts_CN88(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution.ParameterValue)));
            }

            return new GetMultiProblemAccountsModel()
            {
                ProblemAccounts = problemAccounts,
                BackupTimingExecution = savedDataTime
            };
        }

        static IEnumerable<string> InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
            var result = problemAccounts.Select(a => new InsertProblemAccountsModel
            {
                CustId = a.CustID,
                CategoryId = a.ReasonID
            });

            var jsonData = JsonConvert.SerializeObject(result);
            
            return WrapperDataCenterService.Instance.InsertProblemAccounts(jsonData);
        }

        static void InsertProblemAccountToMainDB(List<ProblemAccountForMainDBModel> problemAccounts)
        {
            var specialCustomerClass = WrapperDataCenterService.Instance.GetAllSpecialCustomerClass();
            problemAccounts.RemoveAll(item => specialCustomerClass.Contains(item.CustID));

            if (problemAccounts != null && problemAccounts.Any())
            {
                var countRetry = 0;

                while (countRetry < AppSettings.RetryTime)
                {
                    try
                    {
                        WrapperDataCenterService.Instance.InsertProblemAccountClassificationToMainDB(problemAccounts);
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

        static IEnumerable<ProblemAccountForMainDBModel> GetLatestCategoryForCusts(string custIds)
        {
            var lastestCategories = WrapperDataCenterService.Instance.GetLastestCategory(custIds, true, false)?.Where(c => c.CategoryIdMapping != -99)?.Distinct();

            return lastestCategories;
        }

        static IEnumerable<ProblemCustomerForMainDBModel> GetLatestClassificationForCusts(string custIds)
        {
            //return WrapperDataCenterService.Instance.GetLatestClassification(custIds, true)?.Distinct();
            return null;
        }

        static IEnumerable<ProblemCustomerForMainDBModel> InsertProblemCustomerToMainDB(IEnumerable<ProblemCustomerForMainDBModel> problemCustomers)
        {
            var candidateProblemCustomers = SkipSpecialCustomerClass(problemCustomers);
            if (candidateProblemCustomers != null && candidateProblemCustomers.Any())
            {
                var countRetry = 0;
                while (countRetry < AppSettings.RetryTime)
                {
                    try
                    {
                        WrapperDataCenterService.Instance.InsertProblemCustomerClassificationToMainDB(candidateProblemCustomers.ToList());
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

            return candidateProblemCustomers;
        }
        static IEnumerable<ProblemCustomerForMainDBModel> SkipSpecialCustomerClass(IEnumerable<ProblemCustomerForMainDBModel> problemCustomers)
        {
            if (problemCustomers != null)
            {
                var specialCustomerClass = WrapperDataCenterService.Instance.GetAllSpecialCustomerClass();
                if (specialCustomerClass != null && specialCustomerClass.Any())
                {
                    return problemCustomers.Where(item => !specialCustomerClass.Contains(item.CustID));
                }
            }

            return problemCustomers;
        }
        static DBResult UpdateAppliedCustomerClassification(IEnumerable<ProblemCustomerForMainDBModel> updatedProblemCustomers)
        {
            if (updatedProblemCustomers != null && updatedProblemCustomers.Any())
            {
                var result = updatedProblemCustomers.Select(a => new UpdateAppliedCustomerClassificationModel
                {
                    CustId = a.CustID,
                    ScannedTime = a.ScannedTime
                });

                var jsonData = JsonConvert.SerializeObject(result);

                return WrapperDataCenterService.Instance.UpdateAppliedCustomerClassification(jsonData);
            }

            return new DBResult();
        }
    }
}
