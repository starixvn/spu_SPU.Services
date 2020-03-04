using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.DCS.Criteria;
using Fanex.NAP.DCS.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.DCS.DataAccess
{
    public class AssociationDataAccess
    {
        public List<T> GetData<T>(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<T>(criteria).ToList();
            }
        }    
    }
}