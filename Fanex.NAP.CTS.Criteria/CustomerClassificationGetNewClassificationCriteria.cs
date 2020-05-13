using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class CustomerClassificationGetNewClassificationCriteria : NonQueryCommand
    {
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CustomerClassification_GetNewClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
