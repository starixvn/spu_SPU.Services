using Fanex.Data;
using Fanex.NAP.DCS.Criteria;
using Fanex.NAP.DCS.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.DCS.DataAccess
{
    public class StagingRawTransactionDataAccess
    {
        public List<StagingRawTransactionModel> GetStagingRawTrans(int isProcessed, int numberOfTicket, int numberOfBatch)
        {
            var criteria = new GetRawTransactionCriteria
            {
                IsProcessed = isProcessed,
                NumberOfTickets = numberOfTicket,
                NumberOfBatch = numberOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<StagingRawTransactionModel>(criteria).ToList();
            }
        }

        public List<RawTransactionModel> GetECRawTrans(int batch_size)
        {
            var criteria = new GetECRawTransactionCriteria
            {
              BatchSize= batch_size,
              id= -1
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<RawTransactionModel>(criteria).ToList();
            }
        }
    }
}
