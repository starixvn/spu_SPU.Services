using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Fanex.NAP.SPU.Graph.Core.TestWeb.Controllers
{
    public class ValuesController : ApiController
    {

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
