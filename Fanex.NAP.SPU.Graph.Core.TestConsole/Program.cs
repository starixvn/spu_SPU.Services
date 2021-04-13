using Fanex.NAP.SPU.Graph.Core.Neo4j;
using Fanex.NAP.SPU.Graph.Core.TestConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            var result = DbSessionFactory.Query<CTSCustomer>("TestUpgradeNeo4j", null);


            Console.ReadLine();
        }
    }
}
