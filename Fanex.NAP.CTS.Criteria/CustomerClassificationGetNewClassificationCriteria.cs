using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class CustomerClassificationGetNewClassificationCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GetNewClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
