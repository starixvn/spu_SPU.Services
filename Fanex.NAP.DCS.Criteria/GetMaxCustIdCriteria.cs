namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data.Repository;

    public class GetMaxCustIdCriteria : CriteriaBase
    {

        public override string GetSettingKey()
        {
            return "CTS_DC_GetMaxCustIdCTSCustomer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
