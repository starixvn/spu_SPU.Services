namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateDangerLevelCriteria : NonQueryCommand
    {

        [SpParam(SqlParamName = "CustList")]
        public string CustList { get; set; }

        [SpParam(SqlParamName = "DangerType")]
        public int DangerType { get; set; }

        [SpParam(SqlParamName = "ErrCode")]
        public int ErrorCode { get; set; }

        [SpParam(SqlParamName = "ErrMsg")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerUpdDanger";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
