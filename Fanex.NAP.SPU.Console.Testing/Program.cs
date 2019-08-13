using System;
using System.Data.SqlClient;
using System.IO;

namespace Fanex.NAP.SPU.Console.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

            File.AppendAllText(filePath, "Executed at " + DateTime.Now + Environment.NewLine);

            //try
            //{
            //    OpenSqlConnection();

            //}
            //catch (Exception ex)
            //{
            //    System.Console.WriteLine(ex.Message);
            //}

            //System.Console.ReadKey();
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
    }
}
