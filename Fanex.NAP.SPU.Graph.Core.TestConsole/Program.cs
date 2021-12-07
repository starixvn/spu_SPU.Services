using Fanex.NAP.SPU.Graph.Core.Neo4j;
using Fanex.NAP.SPU.Graph.Core.TestConsole.Models;
using System;
using System.Collections.Generic;
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

            for (int i = 0; i < 60000000; i++)
            {
                DbSessionFactory.Execute("FillMissingAssDev");
                Console.WriteLine($"Batch {i}");
            }


            Console.ReadLine();
        }
    }
}
