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

    class Program
    {
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

                var dataService = new DataCenterService();
                var batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["Batch-Size"]);
                var delayTime = Convert.ToInt32(ConfigurationManager.AppSettings["DelayTimePerBatch"]);
                var filePath = @"import/data.csv";

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
                    Console.WriteLine("In Progess..." + updatedItem/totalItems*100 + "%");
                    Thread.Sleep(delayTime * 1000);
                }

                Console.WriteLine("Update customer class done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

            Console.ReadLine();
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
    }
}
