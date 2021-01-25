namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerInfoFromCTSCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_GetInfoByCustIDList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
