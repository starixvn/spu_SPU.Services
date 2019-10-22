using Fanex.NAP.FS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Fanex.NAP.SPU.Console.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var custid = new List<int> { 1, 2, 3, 11, 12, 13 };

            var result = Test(custid);

            foreach (var item in result)
            {
                System.Console.WriteLine(item.CustId + " - " + item.GroupId);
            }

            System.Console.ReadKey();
        }

        static private string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.
     ConnectionStrings["DBFS.bodb_FraudSensor.bodbSPUNet"].ConnectionString;
        }

        private static void OpenSqlConnection()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                System.Console.WriteLine("State: {0}", connection.State);
                System.Console.WriteLine("ConnectionString: {0}",
                    connection.ConnectionString);
            }
        }

        private static List<ArbitrageCustomer> Test(List<int> custIds)
        {
            var result = new List<ArbitrageCustomer>();

            for (var i = 1; i < 10; i++)
            {
                result.Add(new ArbitrageCustomer
                {
                    CustId = i,
                    GroupId = i * 10
                });
            }

            if (result != null && result.Any())
            {
                var missingCustId = custIds.Where(x => !result.Any(z => z.CustId == x)).ToList();

                if (missingCustId != null && missingCustId.Any())
                {
                    var maxGroupId = result.Max(x => x.GroupId);

                    missingCustId.Select(x => new { missingCustId = x, missingGroupId = ++maxGroupId })
                        .ToList()
                        .ForEach(x => result.Add(new ArbitrageCustomer
                        {
                            CustId = x.missingCustId,
                            GroupId = x.missingGroupId
                        }));
                }

                return result;
            }

            return new List<ArbitrageCustomer>();
        }
    }
}
