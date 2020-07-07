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
                var dataCenterService = new DataCenterService();
                var newClassification = new List<CustomerClassificationModelMoreInfo>();
                int insertedRows = 0;

                do
                {
                    newClassification = dataCenterService.GetNewClassification();
                    if (newClassification != null && newClassification.Any())
                    {
                        var tasks = new List<Task>();
                        var newClassificationChunk = newClassification.Chunk(batchSize).ToList();

                        for (int i = 0; i < newClassificationChunk.Count; i++)
                        {
                            string jsonData = JsonConvert.SerializeObject(newClassificationChunk[i]);
                            dataCenterService.AutoTagClassification(jsonData);

                            var custIdList = string.Join(",", newClassificationChunk[i].Select(c => c.CustId).Distinct());
                            dataCenterService.UpdateNewClassification(custIdList);
                        }
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
