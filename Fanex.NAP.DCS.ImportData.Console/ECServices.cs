using Fanex.Data;
using System.Collections.Generic;

namespace Fanex.NAP.DCS.ImportData.Console
{
    public class ECServices
    {
        public IEnumerable<ECRawTransEntity> GetECRawTrans(int sizeOfBatch)
        {
            var criteria = new GetECRawTransCriteria
            {
                NumberOfTickets = sizeOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ECRawTransEntity>(criteria);
            }
        }
    }
}
