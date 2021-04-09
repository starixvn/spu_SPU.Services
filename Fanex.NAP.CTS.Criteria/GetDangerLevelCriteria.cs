namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDangerLevelCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "ip_TypeID")]
        public int TypeID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_DangerLevel_Get";

        public override bool IsValid()
            => true;
    }
}
