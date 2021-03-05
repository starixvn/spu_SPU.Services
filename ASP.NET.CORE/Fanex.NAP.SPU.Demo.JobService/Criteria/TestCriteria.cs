namespace Fanex.NAP.SPU.Demo.JobService.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TestCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParamId")]
        public int ParamID { get; set; }

        public override string GetSettingKey()
        {
            return "spname";
        }

        public override bool IsValid()
        {
            return true;
        }
    }

}
