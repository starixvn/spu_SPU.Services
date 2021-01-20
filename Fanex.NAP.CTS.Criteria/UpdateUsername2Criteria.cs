namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class UpdateUsername2Criteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Customer")]
        public string CustomerJson { get; set; }

        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_UpdateUserName2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
