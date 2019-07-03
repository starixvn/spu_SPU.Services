using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Criteria
{
    public class DelayGoalPreconditionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_FraudCase_DelayGoal_PreCondition";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
