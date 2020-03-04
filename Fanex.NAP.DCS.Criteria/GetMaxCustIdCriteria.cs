namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMaxCustIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsCustSub")]
        public bool IsCustSub { get; set; }

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
