namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RealtimeUpdateLastScanTimeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "@IsGeneralCategory")]
        public string IsGeneralCategory { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_UpdateLastScanTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
