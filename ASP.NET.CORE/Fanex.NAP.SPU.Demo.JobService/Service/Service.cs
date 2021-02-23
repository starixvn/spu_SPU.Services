namespace Fanex.NAP.SPU.Demo.JobService.Service
{
    using Fanex.Data;
    using Fanex.NAP.SPU.Demo.JobService.Criteria;    
    using System.Collections.Generic;
    using System.Linq;    

    public class Service : IService
    {
        public IEnumerable<object> GetPreviousTimingExecution(int paramId)
        {
            var criteria = new TestCriteria
            {
                ParamID = paramId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<object>(criteria).ToList();
            }
        }
    }
}
