namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RealtimeUpdateLastScanTimeCriteria : NonQueryCommand
    {
        public bool IsGeneralCategory { get; set; }

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
