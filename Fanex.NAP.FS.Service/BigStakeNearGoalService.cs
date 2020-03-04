using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Criteria;
    using System.Collections.Generic;

    public class BigStakeNearGoalService
    {
        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new BigStakeNearGoalPreconditionCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }

        public void Rule(string matchIds)
        {
            var repository = new DynamicRepository();

            var criteria = new BigStakeNearGoalRuleCommand()
            {
                MatchIds = matchIds
            };

            repository.Execute(criteria);
        }
    }
}
