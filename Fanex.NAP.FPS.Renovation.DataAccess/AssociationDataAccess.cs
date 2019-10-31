using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.FPS.Renovation.Criteria;
using Fanex.NAP.FPS.Renovation.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.FPS.Renovation.DataAccess
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