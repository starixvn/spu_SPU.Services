namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetECRawTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ip_ID")]
        public int id { get; set; }

        public override string GetSettingKey()
        {
            return "EC_ProcessedTransaction180";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
