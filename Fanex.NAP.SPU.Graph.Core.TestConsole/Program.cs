using Fanex.NAP.SPU.Graph.Core.Neo4j;
using Fanex.NAP.SPU.Graph.Core.TestConsole.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fanex.NAP.SPU.Graph.Core.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DbSessionFactory.Init()
                .RegisterConfigs("GraphDB")
                .RegisterXmlFile("~/App_Data", "Script.xml")
                .RegisterDriverFactory(new Neo4jDriverFactory())
                .RegisterCommandBuilder(new Neo4jCommandBuilder("~/App_Data/scripts"))
                .RegisterCommandExecutor(new Neo4jExecutor())
                .Run();
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    Console.WriteLine($"Batch {i} start");
                    stopwatch.Restart();
                    DbSessionFactory.Execute("ArchiveCustomer");
                    stopwatch.Stop();
                    Console.WriteLine("Batch {1} - Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds, i);
                }

                //for (int i = 0; i < 1000; i++)
                //{
                //    stopwatch.Restart();
                //    DbSessionFactory.Execute("InitDataLocal");
                //    stopwatch.Stop();
                //    Console.WriteLine("Batch {1} - Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds, i);
                //    if (stopwatch.ElapsedMilliseconds < 100)
                //    {
                //        break;
                //    }
                //}
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw;
            }

            Console.WriteLine($"Done");
            Console.ReadLine();
        }
    }
}
