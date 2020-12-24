namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetLastestClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIds { get; set; }       

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_GetLatestClassification";

        public override bool IsValid()
            => true;
    }
}
