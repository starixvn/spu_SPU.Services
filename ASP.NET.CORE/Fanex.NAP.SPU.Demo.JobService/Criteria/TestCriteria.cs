namespace Fanex.NAP.SPU.Demo.JobService.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TestCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParamID")]
        public int ParamID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetSystemParameterById";
        }

        public override bool IsValid()
        {
            return true;
        }
    }

}
