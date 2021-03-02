namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;

    public class GetAssociationProblemAccountForCustomerClassificationCriteria : Data.Repository.CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListCustID")]
        public string CustIdList { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_GetTaggingByAssociationWithPA";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
