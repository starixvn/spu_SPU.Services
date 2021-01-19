namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetLastInsertCustCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_IsCustSub")]
        public bool IsCustSub { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_GetLastInsertCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}