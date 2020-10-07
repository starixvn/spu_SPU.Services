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
                var dataCenterService = new DataCenterService();
                var newClassification = new List<CustomerClassificationModelMoreInfo>();

                int insertedRows = 0;

                do
                {
                    newClassification = dataCenterService.GetNewClassification();

                    var detailedClassification = newClassification?.Where(c => !c.IsGeneralCategory);
                    var generalClassification = newClassification?.Where(c => c.IsGeneralCategory);
                    var newMemberCategories = newClassification?.Where(c => !c.IsGeneralCategory && c.CategoryId == 2);

                    InsertDetailClassificationCustsToCTS(detailedClassification, dataCenterService, batchSize);
                    InsertGeneralClassificationCusts(generalClassification, dataCenterService, batchSize);
                    InsertNewMemberCategoriesToMainDB(newMemberCategories, dataCenterService);

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

        static void InsertDetailClassificationCustsToCTS(IEnumerable<CustomerClassificationModelMoreInfo> detailedClassification, DataCenterService dataService, int batchSize)
        {
            if (detailedClassification == null || !detailedClassification.Any())
            {
                return;
            }

            var tasks = new List<Task>();
            var newInternalClassificationChunk = detailedClassification.Chunk(batchSize).ToList();

            for (int i = 0; i < newInternalClassificationChunk.Count; i++)
            {
                string jsonData = JsonConvert.SerializeObject(newInternalClassificationChunk[i]);
                dataService.AutoTagClassification(jsonData);

                var custIdList = string.Join(",", newInternalClassificationChunk[i].Select(c => c.CustId).Distinct());
                dataService.UpdateAppliedStatusClassification(custIdList, false);
            }
        }

        static void InsertGeneralClassificationCusts(IEnumerable<CustomerClassificationModelMoreInfo> generalClassification, DataCenterService dataService, int batchSize)
        {
            if (generalClassification == null || !generalClassification.Any())
            {
                return;
            }

            var tasks = new List<Task>();
            var custGeneralCategoryForMainDB = new List<CustomerGeneralClassificationModel>();
            var newGeneralClassificationChunk = generalClassification.Chunk(batchSize).ToList();

            for (int i = 0; i < newGeneralClassificationChunk.Count; i++)
            {
                string jsonData = JsonConvert.SerializeObject(newGeneralClassificationChunk[i]);
                var custForMainDB = dataService.InsertMainCustomerClassification(jsonData);

                if (custForMainDB != null && custForMainDB.Any())
                {
                    custGeneralCategoryForMainDB.AddRange(custForMainDB);
                }

                var custIdList = string.Join(",", newGeneralClassificationChunk[i].Select(c => c.CustId).Distinct());
                dataService.UpdateAppliedStatusClassification(custIdList, true);
            }

            InsertGeneralClassificationCustToMainDB(custGeneralCategoryForMainDB, dataService);
        }

        static void InsertGeneralClassificationCustToMainDB(List<CustomerGeneralClassificationModel> custGeneralCategoryForMainDB, DataCenterService dataService)
        {
            if (custGeneralCategoryForMainDB == null || !custGeneralCategoryForMainDB.Any())
            {
                return;
            }

            var specialCustomerClass = dataService.GetAllSpecialCustomerClass();
            custGeneralCategoryForMainDB.RemoveAll(item => specialCustomerClass.Contains(item.CustId));

            var custGeneralCategoryInRuleForMainDB = custGeneralCategoryForMainDB?.Where(c => c.CategoryIdMapping != -99).ToList();

            if (custGeneralCategoryInRuleForMainDB == null || !custGeneralCategoryInRuleForMainDB.Any())
            {
                return;
            }

            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    dataService.UpdateCustomerGeneralClassificationtoMainDB(custGeneralCategoryInRuleForMainDB);
                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"AutoTag Customer Classification - Update General Classification Customer to MainDB service - Start Exception {logMessage}");
                    countRetry += 1;
                }
            }
        }

        static void InsertNewMemberCategoriesToMainDB(IEnumerable<CustomerClassificationModelMoreInfo> newMemberClassification, DataCenterService dataService)
        {
            if(newMemberClassification == null || !newMemberClassification.Any())
            {
                return;
            }

            var customerClass = dataService.GetCustomerClassByCustId(string.Join(",", newMemberClassification?.Select(c => c.CustId)));

            var newMembers = customerClass?.Where(c => c.CustomerClass == null);

            var tranformNewMemberHelper = new HeplerTranformCusClassificationToCustDB();
            tranformNewMemberHelper.InsertNewMemberCategoryToMainDB(newMembers, true);
        }
    }
}
