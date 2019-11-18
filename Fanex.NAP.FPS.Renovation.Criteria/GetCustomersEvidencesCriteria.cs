using Fanex.Data.Repository;

namespace Fanex.NAP.DCS.Criteria
{
    public class GetCustomersEvidencesCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public string Evidences { get; set; }

        public override string GetSettingKey()
        {
            return "DC_GetEvidencesByCustomers";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
