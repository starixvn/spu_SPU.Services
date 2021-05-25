using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Fanex.NAP.SPU.Graph.Core.TestWeb.Controllers
{
    public class ValuesController : ApiController
    {
        static IDriver driver;

        public ValuesController()
        {
            //var configString = "bolt://10.18.200.72:7687;ctsapi;@vnspu#";
            //var settings = configString.Split(';');
            //driver = GraphDatabase.Driver(settings[0], AuthTokens.Basic(settings[1], settings[2]));
        }
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
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
            var a = DbSessionFactory.Query<int>("GetAssociationReportByDevice_CountTotal", param).FirstOrDefault();
            return a.ToString();
        }
        //public string Get(int id)
        //{
        //    string fresult = string.Empty;
        //    string cmd = @"
        //                        USE ctslocal
        //                        MATCH (sub:Subscriber)<-[:BELONG_TO_SUB]-(c:FlaggedCustomer)-[:HAS_STATUS]->(st:StaticList),(c1:CTSCustomer)-[r:SEEN_ACCOUNT]-(c)-[:HAS_EVIDENCE]->(e:Evidence), (c)-[:HAS_CATEGORY]->(cate:CustomerCategory)
        //                        WHERE sub.SubscriberID IN $ip_SubscriberList
        //                            AND st.ItemID IN $ip_AccountStatus AND st.ListID = 1
        //                            AND e.EvidenceID IN $ip_EvidenceID
        //                            AND r.FirstSeenDate >= datetime($ip_AssCreatedFrom) AND r.FirstSeenDate  < datetime($ip_AssCreatedTo)
        //                            AND (($ip_AssStatus = 0) 
        // 	                                 OR ($ip_AssStatus = 1 AND NOT EXISTS ((c)-[:HAS_EXCEPTION]-(c1)))
        // 	                                 OR ($ip_AssStatus = 2 AND EXISTS ((c)-[:HAS_EXCEPTION]-(c1)))) 
        //                        RETURN COUNT(DISTINCT c) AS TotalItem";

        //    var param = new
        //    {
        //        ip_AssCreatedFrom = new DateTime(2020, 9, 1),
        //        ip_AssCreatedTo = new DateTime(2020, 9, 4),
        //        ip_SubscriberList = new List<int>() { 168, 104 },
        //        ip_AccountStatus = new List<int>() { 1, 11 },
        //        ip_EvidenceID = new List<int>() { 41, 52, 55, 56, 57, 58, 59, 60, 61, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 53, 54, 60, 77, 78, 79, 80, 81, 82, 83, 84, 8 },
        //        ip_AssStatus = 0,
        //        ip_TakeRows = 50,
        //        ip_SkipRows = 0
        //    };

        //    //var driver = GraphDatabase.Driver(settings[0], AuthTokens.Basic(settings[1], settings[2]));
        //    var session = driver.AsyncSession();
        //    var taskResult = session.WriteTransactionAsync(async tx =>
        //    {
        //        var result = await tx.RunAsync(cmd, param);
        //        var tempData = await result.ToListAsync(r => r.As<IRecord>());
        //        fresult = tempData.FirstOrDefault().Values["TotalItem"].ToString();
        //    });

        //    taskResult.Wait();
        //    session.CloseAsync();
        //    return fresult;
        //}

        public string TestBasic()
        {
            string cmd = @"
                                USE ctslocal
                                MATCH (sub:Subscriber)<-[:BELONG_TO_SUB]-(c:FlaggedCustomer)-[:HAS_STATUS]->(st:StaticList),(c1:CTSCustomer)-[r:SEEN_ACCOUNT]-(c)-[:HAS_EVIDENCE]->(e:Evidence), (c)-[:HAS_CATEGORY]->(cate:CustomerCategory)
                                WHERE sub.SubscriberID IN $ip_SubscriberList
                                    AND st.ItemID IN $ip_AccountStatus AND st.ListID = 1
                                    AND e.EvidenceID IN $ip_EvidenceID
                                    AND r.FirstSeenDate >= datetime($ip_AssCreatedFrom) AND r.FirstSeenDate  < datetime($ip_AssCreatedTo)
                                    AND (($ip_AssStatus = 0) 
         	                                 OR ($ip_AssStatus = 1 AND NOT EXISTS ((c)-[:HAS_EXCEPTION]-(c1)))
         	                                 OR ($ip_AssStatus = 2 AND EXISTS ((c)-[:HAS_EXCEPTION]-(c1)))) 
                                RETURN COUNT(DISTINCT c) AS TotalItem";
            var configString = "bolt://10.18.200.72:7687;ctsapi;@vnspu#";
            var settings = configString.Split(';');
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
            var driver = GraphDatabase.Driver(settings[0], AuthTokens.Basic(settings[1], settings[2]));
            var session = driver.AsyncSession();
            var taskResult = session.WriteTransactionAsync(async tx =>
            {
                var result = await tx.RunAsync(cmd, param);
                var tempData = await result.ToListAsync(r => r.As<IRecord>());
                //data = tempData.Select(ParseRecord<T>).ToArray();

                //var counters = result.ConsumeAsync().Result.Counters;
                //affectedNodes = counters.NodesCreated + counters.NodesDeleted;
                //affectedRelationship = counters.RelationshipsCreated + counters.RelationshipsDeleted;
            });

            return "".ToString();
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
