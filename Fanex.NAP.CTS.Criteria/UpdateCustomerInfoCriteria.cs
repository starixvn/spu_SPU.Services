namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateCustomerInfoCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string Customers { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_UpdateInfoComplete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
