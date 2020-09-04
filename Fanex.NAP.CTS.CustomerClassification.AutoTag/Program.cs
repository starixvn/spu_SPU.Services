using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Services;
using Fanex.NAP.FS.Core;
using Fanex.NAP.SPU.Core;
using Newtonsoft.Json;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.CustomerClassification.AutoTag
{
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

            try
            {
                LogService.Info("Start AutoTag Customer Classification ");
                var dataCenterService = new DataCenterService();
                var newClassification = new List<CustomerClassificationModelMoreInfo>();
                int insertedRows = 0;

                do
                {
                    newClassification = dataCenterService.GetNewClassification();
                    LogService.Info("GetNewClassification for AutoTag Cus Classsification done with" + newClassification.Count());

                    var internalClassification = newClassification?.Where(c => !c.IsGeneralCategory);
                    var generalClassification = newClassification?.Where(c => c.IsGeneralCategory);

                    if (internalClassification != null && internalClassification.Any())
                    {
                        Logger.Log.Info("Start internal classification");
                        var tasks = new List<Task>();
                        var newInternalClassificationChunk = internalClassification.Chunk(batchSize).ToList();

                        for (int i = 0; i < newInternalClassificationChunk.Count; i++)
                        {
                            string jsonData = JsonConvert.SerializeObject(newInternalClassificationChunk[i]);
                            dataCenterService.AutoTagClassification(jsonData);

                            var custIdList = string.Join(",", newInternalClassificationChunk[i].Select(c => c.CustId).Distinct());
                            dataCenterService.UpdateAppliedStatusClassification(custIdList, false);
                        }

                        Logger.Log.Info("End internal classification successfully");
                    }

                    if (generalClassification != null && generalClassification.Any())
                    {
                        Logger.Log.Info("Start general classification");

                        var tasks = new List<Task>();
                        var custGeneralCategoryForMainDB = new List<CustomerGeneralClassificationModel>();
                        var newGeneralClassificationChunk = generalClassification.Chunk(batchSize).ToList();

                        for (int i = 0; i < newGeneralClassificationChunk.Count; i++)
                        {
                            string jsonData = JsonConvert.SerializeObject(newGeneralClassificationChunk[i]);
                            var custForMainDB = dataCenterService.InsertMainCustomerClassification(jsonData);

                            if(custForMainDB != null && custForMainDB.Any())
                            {
                                custGeneralCategoryForMainDB.AddRange(custForMainDB);
                            }

                            var custIdList = string.Join(",", newGeneralClassificationChunk[i].Select(c => c.CustId).Distinct());
                            dataCenterService.UpdateAppliedStatusClassification(custIdList, true);
                        }

                        if (custGeneralCategoryForMainDB != null && custGeneralCategoryForMainDB.Any())
                        {
                            var custGeneralCategoryInRuleForMainDB = custGeneralCategoryForMainDB.Where(c => c.CategoryIdMapping != -1).ToList();

                            if (custGeneralCategoryInRuleForMainDB != null && custGeneralCategoryInRuleForMainDB.Any())
                            {
                                var countRetry = 0;

                                while (countRetry < AppSettings.RetryTime)
                                {
                                    try
                                    {
                                        Logger.Log.Info("AutoTag Customer Classification - Update Customer General Classification to MainDB started with " + custGeneralCategoryInRuleForMainDB.Count());
                                        dataCenterService.UpdateCustomerGeneralClassificationtoMainDB(custGeneralCategoryInRuleForMainDB);
                                        countRetry = AppSettings.RetryTime + 1;
                                        Logger.Log.Info("AutoTag Customer Classification - Update Customer General Classification to MainDB finished");
                                    }
                                    catch (Exception ex)
                                    {
                                        var logMessage = new LogMessage(ex.Message)
                                         .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                         .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                                        LogService.Error($"AutoTag Customer Classification - Update Customer General Classification to MainDB service - Start Exception {logMessage}");
                                        countRetry += 1;
                                    }
                                }
                            }
                        }

                        Logger.Log.Info("End general classification successfully");
                    }
                } while (newClassification != null && newClassification.Any());

                insertedRows += newClassification.Count;
                
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
