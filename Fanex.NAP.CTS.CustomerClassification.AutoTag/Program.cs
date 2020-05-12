using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.CTS.Services;
using Fanex.NAP.SPU.Core;
using Newtonsoft.Json;
using SharpRaven;
using System;
using System.Collections.Generic;
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

            try
            {
                var dataCenterService = new DataCenterService();
                var newClassification = dataCenterService.GetNewClassification();

                if(newClassification != null && newClassification.Any())
                {
                    string jsonData = JsonConvert.SerializeObject(newClassification);

                    dataCenterService.AutoTagClassification(jsonData);

                    dataCenterService.UpdateNewClassification();
                }
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
