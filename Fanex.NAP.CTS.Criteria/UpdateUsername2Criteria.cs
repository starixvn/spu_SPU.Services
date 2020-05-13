namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class UpdateUsername2Criteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_UpdateUsername2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
