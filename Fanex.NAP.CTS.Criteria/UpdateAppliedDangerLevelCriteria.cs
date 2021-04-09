namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateAppliedDangerLevelCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ProcessedCustList")]
        public string CustJson { get; set; }

        [SpParam(SqlParamName = "ip_TypeID")]
        public int TypeID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DangerLevel_UpdateFlag";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
