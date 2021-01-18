namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ReTransformAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Customer")]
        public string customer { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_InsertComplete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
