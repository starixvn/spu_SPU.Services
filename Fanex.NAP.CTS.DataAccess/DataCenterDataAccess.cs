using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.CTS.Criteria;
using Fanex.NAP.CTS.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.DataAccess
{
    public class DataCenterDataAccess
    {
        public int InsertAccount(string accountJson)
        {
            var repository = new DynamicRepository();

            var criteria = new InsertAccountCriteria
            {
                AccountJson = accountJson
            };

            return repository.Execute(criteria);
        }

        public void TransformAccountCompleted(string accountJson)
        {
            var repository = new DynamicRepository();

            var criteria = new TransformAccountCompletedCriteria
            {
                AccountJson = accountJson
            };

            repository.Execute(criteria);
        }
    }
}
