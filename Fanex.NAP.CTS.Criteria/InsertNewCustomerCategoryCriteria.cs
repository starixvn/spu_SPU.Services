namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TransformCustomerFromMainDbCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
