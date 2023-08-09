using Fanex.NAP.DCS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.NAP.DCS.Criteria;
using Fanex.Data.Repository;
using Fanex.Data;

namespace Fanex.NAP.DCS.DataAccess
{
    public class CommonDataAccess
    {
        public IEnumerable<IpInfoModel> GetIpInfoList(string ipAddressList)
        {
            var criteria = new IpInfoCriteria
            {
                IpAddressList = ipAddressList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<IpInfoModel>(criteria);
            }
        }
    }
}
