namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerCategoryLastedCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string IDList { get; set; }

        [SpParam(SqlParamName = "ip_IsUsingCustID")]
        public bool IsUsingCustID { get; set; }

        [SpParam(SqlParamName = "ip_IsExcludedDetailedCategory")]
        public bool IsExcludedDetailedCategory { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_GetLatestCategory";

        public override bool IsValid()
            => true;
    }
}
