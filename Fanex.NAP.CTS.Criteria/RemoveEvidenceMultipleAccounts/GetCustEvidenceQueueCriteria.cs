using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.RemoveMultipleEvidences
{
    public class GetCustEvidenceQueueCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidenceQueue_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
