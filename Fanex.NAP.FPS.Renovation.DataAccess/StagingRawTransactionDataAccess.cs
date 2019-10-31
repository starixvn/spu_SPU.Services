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
    }
}
