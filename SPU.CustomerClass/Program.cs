namespace SPU.CustomerClass
{
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.Data.MySql;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Globalization;

    class Program
    {
        readonly static DataCenterService dataService = new DataCenterService();

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting...!");
                DbSettingProviderManager
                     .StartNewSession()
                     .WithSqlServer()
                     .WithMySql()
                     .UseDefaultDbSettingProvider(resourcePath: AppSettings.AppDataSetting)
                     .UseDefaultConnectionStringProvider()
                     .Run();

                var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);
                var delayTime = Convert.ToInt32(ConfigurationManager.AppSettings["DelayTimePerBatch"]);
                var filePath = @"import/data.csv";
                int inputFunction = Welcome();

                switch (inputFunction)
                {
                    case 1:
                        UpdateCustomerClass(filePath, batchSize, delayTime);
                        break;
                    case 2:
                        UpdateRobotUser();
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

            Console.ReadLine();
        }

        private static void UpdateRobotUser()
        {
            Console.WriteLine("Start update robot user...");

            var robotUsers = new List<RobotUser>
            {
                new RobotUser() { CustId = 47005079, RobotDate = DateTime.ParseExact("12/8/2020 15:56:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) },
                new RobotUser() { CustId = 46412006, RobotDate = DateTime.ParseExact("11/6/2020 4:57:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) },
                new RobotUser() { CustId = 46412038, RobotDate = DateTime.ParseExact("11/6/2020 2:23:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) },
                new RobotUser() { CustId = 46412066, RobotDate = DateTime.ParseExact("11/6/2020 2:22:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) },
                new RobotUser() { CustId = 46411771, RobotDate = DateTime.ParseExact("11/6/2020 15:03:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) },
                new RobotUser() { CustId = 46411773, RobotDate = DateTime.ParseExact("11/6/2020 6:11:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) },
                new RobotUser() { CustId = 47042138, RobotDate = DateTime.ParseExact("12/8/2020 16:11:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) },
                new RobotUser() { CustId = 46864627, RobotDate = DateTime.ParseExact("11/28/2020 23:42:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) },
                new RobotUser() { CustId = 46877168, RobotDate = DateTime.ParseExact("12/9/2020 22:33:00","MM/d/yyyy H:mm:ss",CultureInfo.InvariantCulture) }
            };

            if (robotUsers.Any())
            {
                var robotUsersChunks = robotUsers.Chunk(150).ToList();
                foreach (var robotUserList in robotUsersChunks)
                {
                    var custClasses = dataService.InsertRobotUsers(string.Join(",", robotUserList.Select(c => c.CustId)));
                    dataService.UpdateCustomerGeneralClassificationtoMainDB(custClasses);
                    Thread.Sleep(300);
                }
            }

            Console.WriteLine("Update robot user done!");
        }

        private static void UpdateCustomerClass(string filePath, int batchSize, int delayTime)
        {
            Console.WriteLine("Reading data...!");
            List<CustomerClassModel> listCustClass = ReadDataFromCsvFile(filePath);
            int totalItems = listCustClass.Count;
            int updatedItem = 0;
            var listCustClassChunk = listCustClass.Chunk(batchSize).ToList();

            Console.WriteLine("Reading data done! - " + listCustClass.Count + " customers");
            Console.WriteLine("Updating data...!");
            foreach (var listCust in listCustClassChunk)
            {
                Console.WriteLine("Updating data...! - Total count: " + listCust.Count());
                dataService.UpdateCustomerGeneralClassificationtoMainDB(listCust);
                updatedItem += listCust.Count();
                Console.WriteLine("In Progess..." + updatedItem / totalItems * 100 + "%");
                Thread.Sleep(delayTime * 1000);
            }

            Console.WriteLine("Update customer class done!");
        }

        private static List<CustomerClassModel> ReadDataFromCsvFile(string filePath)
        {
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

        private static int Welcome()
        {
            int inputFromUser = 0;
            Console.WriteLine("WELCOME TO BACKDOOR TOOL!");
            Console.WriteLine("********** Please use with care **********");
            Console.WriteLine("*** What do you want to do ?");
            Console.WriteLine("1. Update customer class");
            Console.WriteLine("2. Get robot user");
            Console.WriteLine("Choose function and press ENTER.......");
            inputFromUser = Convert.ToInt32(Console.ReadLine());

            return inputFromUser;
        }
    }
}
