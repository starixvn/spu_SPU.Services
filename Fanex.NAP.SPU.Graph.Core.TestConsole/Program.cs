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

            var result = new GraphAssociationReportPaging();
            var param = new
            {
                ip_AssCreatedFrom = new DateTime(2020, 9, 1),
                ip_AssCreatedTo = new DateTime(2020, 9, 4),
                ip_SubscriberList = new List<int>() { 168, 104 },
                ip_AccountStatus = new List<int>() { 1, 11 },
                ip_EvidenceID = new List<int>() { 41, 52, 55, 56, 57, 58, 59, 60, 61, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 53, 54, 60, 77, 78, 79, 80, 81, 82, 83, 84, 8 },
                ip_AssStatus = 0,
                ip_TakeRows = 50,
                ip_SkipRows = 0
            };

            do
            {
                try
                {
                    //result.TotalItems = DbSessionFactory.QuerySingle<int>("CountTotal", param);
                    result.TotalItems = DbSessionFactory.Execute<int>("CountTotal", param).Data.FirstOrDefault();
                    result.AssociationList = DbSessionFactory.Execute<AssociationReportModel>("GetReport", param).Data;
                    Console.WriteLine("result.TotalItems: " + result.TotalItems.ToString());
                    Console.WriteLine("first item result.AssociationList: " + result.AssociationList.FirstOrDefault().Username);
                    Console.WriteLine("query done");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace + "\n" + ex.InnerException.Message);
                }
            } while (int.Parse(Console.ReadLine()) == 1);
            
            Console.ReadLine();
        }
    }
}
