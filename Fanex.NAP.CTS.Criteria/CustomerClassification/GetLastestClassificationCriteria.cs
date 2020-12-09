namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetLastestClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustIDList { get; set; }

        [SpParam(SqlParamName = "ip_IsCustId")]
        public bool IsUsingCustID { get; set; }       

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_GetLatestClassification";

        public override bool IsValid()
            => true;
    }
}
