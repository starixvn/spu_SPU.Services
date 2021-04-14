namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetExistedRobotUserCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "custids")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "RobotList")]
        public string RobotList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_RobotCheckExist";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
