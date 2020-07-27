using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Services;
using Fanex.NAP.SPU.Core;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Fanex.NAP.CTS.AutoTagProblemAccount.Console
{
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

            try
            {
                LogService.Info("Start AutoTag Problem Account");
                System.Console.WriteLine("Start AutoTag Problem Account");
                var previousTimingExecution = ReadSetting(Constants.SAVED_PREVIOUS_TIMING_EXECUTION);

                var problemAccounts = new List<ProblemAccountsModel>();

                if (string.IsNullOrEmpty(previousTimingExecution))
                {
                    problemAccounts = dataService.GetProblemAccounts(Constants.GET_ALL_INITIAL_DATA, DateTime.Now).ToList();
                }
                else
                {
                    problemAccounts = dataService.GetProblemAccounts(Constants.GET_CHANGED_DATA_FROM_DATETIME, DateTime.Parse(previousTimingExecution)).ToList();
                }

                LogService.Info("GetProblemAccounts done");
                System.Console.WriteLine("GetProblemAccounts done");
                AddUpdateAppSettings(Constants.SAVED_PREVIOUS_TIMING_EXECUTION, DateTime.Now.ToString(Constants.CONVERT_TO_SQL_DATETIME));

                if (problemAccounts.Any())
                {
                    var result = problemAccounts.Select(a => new InsertProblemAccountsModel
                    {
                        CustId = a.CustID,
                        CategoryId = a.ReasonID
                    });

                    var jsonData = JsonConvert.SerializeObject(result);

                    dataService.InsertProblemAccounts(jsonData);
                }

                LogService.Info("InsertProblemAccounts done");
                System.Console.WriteLine("InsertProblemAccounts done");
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
                System.Console.WriteLine("Bi loi cmnr");
            }
        }

        private static string ReadSetting(string key)
        {
            System.Console.WriteLine("ReadSetting is running");
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings[key] ?? "";
        }

        private static void AddUpdateAppSettings(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
