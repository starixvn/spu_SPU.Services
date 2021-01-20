namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    using System;

    public class GetCustomersWithNullUsername2Criteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_PreCTSCustID")]
        public Int64 PreCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_GetNULLUserName2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
