namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertMainCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NewClassification")]
        public string NewClassification { get; set; }

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
