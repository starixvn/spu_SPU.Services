namespace Fanex.NAP.CTS.CustomerClassification.AutoTag
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Helper;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static IDataCenterService dataCenterService = new DataCenterService();

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
                
                var newClassification = new List<CustomerClassificationModelMoreInfo>();

                int insertedRows = 0;

                do
                {
                    newClassification = dataCenterService.GetNewClassification();
                    
                    var generalClassifications = newClassification?.Where(c => c.IsGeneralCategory);

                    InsertGeneralClassificationCusts(generalClassifications);
                
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

            Console.WriteLine("CustomerClassification.AutoTag done");
            Console.ReadLine();
        }

        static void InsertGeneralClassificationCusts(IEnumerable<CustomerClassificationModelMoreInfo> generalClassification)
        {
            if (generalClassification == null && generalClassification.Any())
            {
                var custGeneralCategoryForMainDB = new List<CustomerGeneralClassificationModel>();
                var newGeneralClassificationChunk = generalClassification.Chunk(AppSettings.BatchSize).ToList();

                foreach (var listClassification in newGeneralClassificationChunk)
                {
                    try
                    {
                        var jsonData = JsonConvert.SerializeObject(listClassification);

                        // Insert data in CTS
                        var typeCustModel = dataCenterService.InsertMainCustomerClassification(jsonData);

                        var custIdList = string.Join(",", listClassification.Select(c => c.CustId).Distinct());

                        // Update IsApplied in mainDB
                        dataCenterService.UpdateAppliedStatusClassification(custIdList, true);

                        // Get lastest classification in CTS
                        var lastestClassifications = dataCenterService.GetLatestClassification(typeCustModel);

                        // Update applied classification
                        if (lastestClassifications != null && lastestClassifications.Any())
                        {
                            // Update CustomerClass in table custinfo to MainDB
                            dataCenterService.UpdateCustomerGeneralClassificationtoMainDB(lastestClassifications);

                            var updateApplyCCs = lastestClassifications.Select(x => new { x.CustId, x.ScannedTime });

                            jsonData = JsonConvert.SerializeObject(updateApplyCCs);

                            // Update IsAppliedCC = 1 in CTS
                            dataCenterService.UpdateAppliedCustomerClassification(jsonData);
                        }

                        // Feed data to Soon88
                        FeedDataToSoon88Helper.FeedCCToSoon88(lastestClassifications);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }

            }
           
        }

        //static void InsertGeneralClassificationCusts(IEnumerable<CustomerClassificationModelMoreInfo> generalClassification)
        //{
        //    if (generalClassification == null || !generalClassification.Any())
        //    {
        //        return;
        //    }

        //    var custGeneralCategoryForMainDB = new List<CustomerGeneralClassificationModel>();
        //    var newGeneralClassificationChunk = generalClassification.Chunk(AppSettings.BatchSize).ToList();

        //    for (int i = 0; i < newGeneralClassificationChunk.Count; i++)
        //    {

        //        string jsonData = JsonConvert.SerializeObject(newGeneralClassificationChunk[i]);

        //        var custForMainDB = dataCenterService.InsertMainCustomerClassification(jsonData);

        //        if (custForMainDB != null && custForMainDB.Any())
        //        {
        //            custGeneralCategoryForMainDB.AddRange(custForMainDB);
        //        }

        //        var custIdList = string.Join(",", newGeneralClassificationChunk[i].Select(c => c.CustId).Distinct());
        //        dataCenterService.UpdateAppliedStatusClassification(custIdList, true);
        //    }

        //    InsertGeneralClassificationCustToMainDB(custGeneralCategoryForMainDB);
        //}

        static void InsertGeneralClassificationCustToMainDB(List<CustomerGeneralClassificationModel> custGeneralCategoryForMainDB)
        {
            if (custGeneralCategoryForMainDB == null || !custGeneralCategoryForMainDB.Any())
            {
                return;
            }

            var specialCustomerClass = dataCenterService.GetAllSpecialCustomerClass();
            custGeneralCategoryForMainDB.RemoveAll(item => specialCustomerClass.Contains(item.CustId));

            var custGeneralCategoryInRuleForMainDB = custGeneralCategoryForMainDB?.Where(c => c.CategoryIdMapping != -99).ToList();

            if (custGeneralCategoryInRuleForMainDB == null || !custGeneralCategoryInRuleForMainDB.Any())
            {
                return;
            }
            
            var retryTime = AppSettings.RetryTime;

            var listCustClassChunk = custGeneralCategoryInRuleForMainDB.Chunk(AppSettings.BatchSize).ToList();

            foreach (var listCust in listCustClassChunk)
            {
                var isSuccess = false;

                while (!isSuccess && retryTime > 0)
                {
                    try
                    {
                        // Get lastest classification
                        var custIds = string.Join(",", listCust.Select(x => x.CustId));

                        var lastestCusts = listCust.Select(x => new TypeCategoryCustomerModel
                        {
                            CustId = x.CustId,
                            TypeId = 4
                        });

                        var lastestClassifications = dataCenterService.GetLatestClassification(lastestCusts);

                        // Update general classification to MainDB
                        dataCenterService.UpdateCustomerGeneralClassificationtoMainDB(lastestClassifications);

                        // Update applied classification
                        if (lastestClassifications != null && lastestClassifications.Any())
                        {
                            var updateApplyCCs = lastestClassifications.Select(x => new { x.CustId, x.ScannedTime });
                            var jsonData = JsonConvert.SerializeObject(updateApplyCCs);

                            dataCenterService.UpdateAppliedCustomerClassification(jsonData);
                        }

                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        retryTime = retryTime - 1;

                        if (retryTime == 0)
                        {
                            var jsonDataForLog = JsonConvert.SerializeObject(listCust);

                            var logMessage = new LogMessage(ex.Message)
                               .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                               .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                            LogService.Error($"AutoTag Customer Classification service - InsertGeneralClassificationCustToMainDB Exception {logMessage} with listCust: {jsonDataForLog}");
                        }
                    }
                }
            }
        }        

    }
}
