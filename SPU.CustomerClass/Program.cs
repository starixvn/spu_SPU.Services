namespace SPU.CustomerClass
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
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;

    class Program
    {
        readonly static DataCenterService dataService = new DataCenterService();
        readonly static CustomerClassifyService customerClassificationService = new CustomerClassifyService();

        static void Main(string[] args)
        {
            try
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

                Console.WriteLine("Starting...!");

                var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);
                var delayTime = Convert.ToInt32(ConfigurationManager.AppSettings["DelayTimePerBatch"]);
                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"import\data.csv";

                int inputFunction = Welcome();

                switch (inputFunction)
                {
                    case 1:
                        {
                            UpdateCustomerClass(filePath, batchSize, delayTime);
                            break;
                        }

                    case 2:
                        {
                            RescanCustomerClassification(filePath, batchSize, delayTime);
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
#if DEBUG
            Console.ReadLine();
#endif
        }

        private static void UpdateCustomerClass(string filePath, int batchSize, int delayTime)
        {
            Console.WriteLine("Reading data...!");
            List<CustomerClassModel> listCustClass = ReadDataFromCsvFile(filePath);
            decimal totalItems = listCustClass.Count;
            decimal updatedItem = 0;
            decimal percent = 0;
            var listCustClassChunk = listCustClass.Chunk(batchSize).ToList();

            Console.WriteLine("Reading data done! - " + listCustClass.Count + " customers");
            Console.WriteLine("Updating data...!");
            foreach (var listCust in listCustClassChunk)
            {
                Console.WriteLine("Updating data...! - Total count: " + listCust.Count());
                dataService.UpdateCustomerGeneralClassificationtoMainDB(listCust);
                updatedItem += listCust.Count();
                percent = Math.Round(updatedItem / totalItems * 100, 5);
                Console.WriteLine("In Progress..." + percent + "%");
                Thread.Sleep(delayTime * 1000);
            }

            Console.WriteLine("Update customer class done!");
        }

        private static void RescanCustomerClassification(string filePath, int batchSize, int delayTime)
        {
            Console.WriteLine("Reading data...!");
            List<int> custIDs = ReadCustIDsFromCsvFile(filePath);
            decimal totalItems = custIDs.Count;
            decimal updatedItem = 0;
            decimal percent = 0;
            var custIDsChunk = custIDs.Chunk(batchSize).ToList();

            Console.WriteLine("Reading data done! - " + custIDs.Count + " custIds");
            Console.WriteLine("Updating data...!");
            foreach (var custIDsBatch in custIDsChunk)
            {
                Console.WriteLine("Updating data...! - Total count: " + custIDsBatch.Count());
                customerClassificationService.RescanCustomerClassify(custIDsBatch.ToList(), true, true, Constants.FLOWTYPERESCANCC.NEWMEMBER); //NEWMBER for insert new log history
                updatedItem += custIDsBatch.Count();
                percent = Math.Round(updatedItem / totalItems * 100, 5);
                Console.WriteLine("In Progress..." + percent + "%");
                Thread.Sleep(delayTime * 1000);
            }

            Console.WriteLine("Rescan customer classification done!");
        }

        private static List<CustomerClassModel> ReadDataFromCsvFile(string filePath)
        {
            LogService.Info("filePath: " + filePath);
            StreamReader sreader = new StreamReader(filePath);
            var listCustClass = new List<CustomerClassModel>();
            var stringLine = string.Empty;
            string[] values = null;
            while (!sreader.EndOfStream)
            {
                stringLine = sreader.ReadLine();
                values = stringLine.Split(',');
                var custId = values[0].Trim();
                var custClass = values[1].Trim();

                if (!string.IsNullOrEmpty(custId) && !string.IsNullOrEmpty(custClass))
                {
                    listCustClass.Add(new CustomerClassModel()
                    {
                        CustID = int.Parse(custId),
                        CustomerClass = int.Parse(custClass)
                    });
                }
            }

            sreader.Close();
            return listCustClass;
        }

        private static List<int> ReadCustIDsFromCsvFile(string filePath)
        {
            LogService.Info("filePath: " + filePath);
            StreamReader sreader = new StreamReader(filePath);
            var custIds = new List<int>();
            var stringLine = string.Empty;
            string[] values = null;
            while (!sreader.EndOfStream)
            {
                stringLine = sreader.ReadLine();
                values = stringLine.Split(',');
                var custId = values[0].Trim();

                if (!string.IsNullOrEmpty(custId))
                {
                    custIds.Add(int.Parse(custId));
                }
            }

            sreader.Close();
            return custIds;
        }

        private static int Welcome()
        {
            int inputFromUser = 0;
            Console.WriteLine("WELCOME TO BACKDOOR TOOL!");
            Console.WriteLine("********** Please use with care **********");
            Console.WriteLine("*** What do you want to do ?");
            Console.WriteLine("1. Update customer class");
            Console.WriteLine("2. Rescan customer class");
            Console.WriteLine("Choose function and press ENTER.......");
            inputFromUser = Convert.ToInt32(Console.ReadLine());

            return inputFromUser;
        }
    }
}
