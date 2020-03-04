using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.DCS.Criteria;
using Fanex.NAP.DCS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.DataAccess
{
    public class RawTransactionDataAccess
    {
        public int InsertTransaction(string jsonTrans)
        {
            var repository = new DynamicRepository();

            var criteria = new CapturingTransactionCriteria
            {
                RawTrans = jsonTrans   
            };

           return repository.Execute(criteria);
        }

        public int InsertTransaction2(string jsonTrans)
        {
            var repository = new DynamicRepository();

            var criteria = new CapturingTransactionCriteria2
            {
                RawTrans = jsonTrans
            };

            return repository.Execute(criteria);
        }
    }
}
