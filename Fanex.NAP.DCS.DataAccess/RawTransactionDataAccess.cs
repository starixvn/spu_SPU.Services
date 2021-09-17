using Fanex.Data.Repository;
using Fanex.NAP.DCS.Criteria;

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
    }
}
