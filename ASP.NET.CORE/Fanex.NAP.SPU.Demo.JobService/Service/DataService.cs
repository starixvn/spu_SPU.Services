namespace Fanex.NAP.SPU.Demo.JobService.Service
{
    using Fanex.Data;
    using Fanex.NAP.SPU.Demo.JobService.Criteria;    
    using System.Collections.Generic;
    using System.Linq;

    public interface IDataService
    {
        IEnumerable<object> GetData(int paramId);
    }

    public class DataService : IDataService
    {
        public IEnumerable<object> GetData(int paramId)
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
