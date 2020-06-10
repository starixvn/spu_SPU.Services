using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Services;
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
            LogService.Info("Start CustomerClassification.AutoTag");
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
                var dataCenterService = new DataCenterService();
                var newClassification = new List<CustomerClassificationModel>();

                do
                {
                    LogService.Info("CustomerClassification.AutoTag get data");
                    newClassification = dataCenterService.GetNewClassification();
                    LogService.Info("newClassification: " + newClassification.Count);
                    if (newClassification != null && newClassification.Any())
                    {
                        string jsonData = JsonConvert.SerializeObject(newClassification);
                        LogService.Info("jsonData: " + jsonData);
                        dataCenterService.AutoTagClassification(jsonData);

                        var maxCustId = newClassification.Max(x => x.CustId);

                        dataCenterService.UpdateNewClassification(maxCustId);
                        LogService.Info("maxCustId: " + maxCustId);
                    }
                    LogService.Info("CustomerClassification.AutoTag done!");
                }
                while (newClassification != null && newClassification.Any());
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
