namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertMainCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NewClassification")]
        public string NewClassification { get; set; }

        [SpParam(SqlParamName = "ip_IsRescan")]
        public bool IsRescan { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_InsertMainCustomerClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
