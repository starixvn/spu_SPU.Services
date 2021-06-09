namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerClassificationByCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportsJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCustomerClassificationByCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
