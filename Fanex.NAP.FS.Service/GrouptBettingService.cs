namespace Fanex.NAP.FS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Criteria;
    using System.Collections.Generic;

    public class GrouptBettingService
    {
        public IEnumerable<T> PreCondition<T>()
        {
            var criteria = new GroupBettingPreConditionCriteria();
            var repository = new DynamicRepository();

            return repository.Fetch<T>(criteria);
        }

        public void Rule(string matchIds, string accountList)
        {
            var repository = new DynamicRepository();

            var criteria = new GroupBettingRuleCriteria()
            {
                MatchIds = matchIds,
                GroupBettingAccountsXML = accountList
            };

            repository.Execute(criteria);
        }

        public IEnumerable<T> GetGroupBettingCustomer<T>(string evidenceIdList)
        {
            var repository = new DynamicRepository();
            var criteria = new GetCustEvidenceByEviIDCriteria()
            {
                EvidenceId = evidenceIdList
            };

            return repository.Fetch<T>(criteria);
        }

    }
}
