namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class UpdateCustomerInfoCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerInfo")]
        public string Customers { get; set; }

        [SpParam(SqlParamName = "ip_LastUpdateTime")]
        public DateTime LastUpdateTime { get; set; }

        [SpParam(SqlParamName = "ip_LastUpdateCustID")]
        public Int64 LastUpdateCustID { get; set; }

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
