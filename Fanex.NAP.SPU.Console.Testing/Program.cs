using Fanex.Logging;
using Fanex.Logging.Sentry;
using Fanex.NAP.DCS.Core;
using Fanex.NAP.FS.Model;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;
using Sentry;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Timers;

namespace Fanex.NAP.SPU.Console.Testing
{
    class Program
    {
        protected static IUMControlAdapter maintenanceService;

        private static Timer timer;

        static void Main(string[] args)
        {
            var ipv6Number = "0:0:0:0:0:ffff:b4fb:4a8b".Ip2Long().IpNumber;

            System.Console.WriteLine("IpV6->" + ipv6Number);

            var ipV6 = Utilities.Long2DotIP(ipv6Number.ToString());

            System.Console.WriteLine(ipV6);

            var ipv4Number = "180.251.74.139".Ip2Long().IpNumber;

            System.Console.WriteLine("IpV4->" + ipv4Number);

            var ipV4 = Utilities.Long2DotIP(ipv4Number.ToString());

            System.Console.WriteLine(ipV4);

            //LogManager.SetDefaultLogCategory("Testing")
            //        .Use(new List<ILogEngine>
            //        {
            //             new SentryLogging(new SentryEngineOptions
            //             {
            //                 Dsn = new Dsn("http://5db4409de047476aacba9ee1f3c70e26@10.40.40.47:801/63")
            //             })
            //        });

            //maintenanceService = new UMControlAdapter();            

            //timer = new System.Timers.Timer();
            //timer.Interval = 60000;

            //timer.Elapsed += OnTimedEvent;
            //timer.AutoReset = true;
            //timer.Enabled = true;

            System.Console.ReadKey();
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var isUM = maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;

                if (isUM)
                {
                    System.Console.WriteLine("UM is On");
                }
                else
                {
                    System.Console.WriteLine("UM is Off");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

     //   static private string GetConnectionString()
     //   {
     //       return System.Configuration.ConfigurationManager.
     //ConnectionStrings["DBFS.bodb_FraudSensor.bodbSPUNet"].ConnectionString;
     //   }

     //   private static void OpenSqlConnection()
     //   {
     //       string connectionString = GetConnectionString();
     //       using (SqlConnection connection = new SqlConnection())
     //       {
     //           connection.ConnectionString = connectionString;
     //           connection.Open();
     //           System.Console.WriteLine("State: {0}", connection.State);
     //           System.Console.WriteLine("ConnectionString: {0}",
     //               connection.ConnectionString);
     //       }
     //   }

     //   private static List<ArbitrageCustomer> Test(List<int> custIds)
     //   {
     //       var result = new List<ArbitrageCustomer>();

     //       for (var i = 1; i < 10; i++)
     //       {
     //           result.Add(new ArbitrageCustomer
     //           {
     //               CustId = i,
     //               GroupId = i * 10
     //           });
     //       }

     //       if (result != null && result.Any())
     //       {
     //           var missingCustId = custIds.Where(x => !result.Any(z => z.CustId == x)).ToList();

     //           if (missingCustId != null && missingCustId.Any())
     //           {
     //               var maxGroupId = result.Max(x => x.GroupId);

     //               missingCustId.Select(x => new { missingCustId = x, missingGroupId = ++maxGroupId })
     //                   .ToList()
     //                   .ForEach(x => result.Add(new ArbitrageCustomer
     //                   {
     //                       CustId = x.missingCustId,
     //                       GroupId = x.missingGroupId
     //                   }));
     //           }

     //           return result;
     //       }

     //       return new List<ArbitrageCustomer>();
     //   }
    }
}
