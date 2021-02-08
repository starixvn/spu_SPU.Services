namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class UpdateCustInfoCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfo { get; set; }

        [SpParam(SqlParamName = "ip_LastUpdateTime")]
        public DateTime? LastUpdateTime { get; set; }

        [SpParam(SqlParamName = "ip_LastUpdateCustID")]
        public Int64? LastUpdateCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_UpdateCustInfoComplete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
