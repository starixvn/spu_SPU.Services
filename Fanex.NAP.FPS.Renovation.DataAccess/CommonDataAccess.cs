using Fanex.NAP.FPS.Renovation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.NAP.FPS.Renovation.Criteria;
using Fanex.Data.Repository;
using Fanex.Data;

namespace Fanex.NAP.FPS.Renovation.DataAccess
{
    public class CommonDataAccess
    {
        public IpInfoModel GetIpInfo(string ipAddress)
        {
            var criteria = new IpInfoCriteria
            {
                IpAddress = ipAddress
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<IpInfoModel>(criteria);
            }
        }
    }
}
