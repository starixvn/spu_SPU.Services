using Fanex.Data.Repository;
using Fanex.NAP.FS.Criteria;
using System.Collections.Generic;

namespace Fanex.NAP.FS.Service
{
    public class HedgingService
    {
        public IEnumerable<T> PreCondition<T>()
        {
            var criteria = new HedgingPreConditionCriteria();
            var repository = new DynamicRepository();

            return repository.Fetch<T>(criteria);
        }

        public void Rule(string matchIds, string accountList)
        {
            var repository = new DynamicRepository();

            var criteria = new HedgingRuleCriteria()
            {
                MatchIds = matchIds,
                HedgingAccountsXML = accountList
            };

            repository.Execute(criteria);
        }
    }
}
