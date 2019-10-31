using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.FPS.Renovation.Criteria;
using Fanex.NAP.FPS.Renovation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.DataAccess
{
   public class IPRangeDataAccess
    {
        public int UpdateIPRange(DateTime updatedDate)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateIpRangeCriteria
            {                
                UpdatedDate = updatedDate                
            };

            return repository.Execute(criteria);           
        }        

        public List<IpRangeModel> GetIpV6Range(int fromId, int toId)
        {
            var criteria = new GetIpV6RangeCriteria
            {
                FromId = fromId,
                ToId = toId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<IpRangeModel>(criteria).ToList();
            }
        }

        public DateTime GetIPDBVersion()
        {
            var criteria = new GetIPDBVersionCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<DateTime>(criteria);
            }
        }

        public DateTime GetLatestUpdatedDataIPTableLog()
        {
            var criteria = new GetIPTableLogLatestUpdatedDateCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<DateTime>(criteria);
            }
        }       

        public DataResult InsertNewTableLog(DateTime updatedDate)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertIPTableLogCriteria
            {
               UpdatedDate = updatedDate
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }
    }
}
