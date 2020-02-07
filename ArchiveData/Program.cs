using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ArchiveData
{
    public static class Program
    {
        static int noOfTickets = 1000;
        static void Main(string[] args)
        {
            DbSettingProviderManager
                .StartNewSession()
                .WithMySql()
                .UseDefaultDbSettingProvider(resourcePath: "~/App_Data/SPNames.xml")
                .UseDefaultConnectionStringProvider()
                .Run();

            LogManager.SetDefaultLogCategory("Archive-Raw-Data")
                    .Use(new List<ILogEngine>
                    {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn("http://5db4409de047476aacba9ee1f3c70e26@10.99.99.37:801/63")
                         })
                    });

            
            Console.Write("Number of tickets: ");
            noOfTickets = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Archive process is starting.");

            var timer = new Timer(60000);

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();

            Console.ReadKey();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                Run(noOfTickets);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);

                Console.WriteLine(ex.Message);
            }
        }

        static void Run(int noOfTickets)
        {
            var criteria = new ArchiveFPSDataCriteria
            {
                NoOfTickets = noOfTickets
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }
    }
}
