namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ArchiveTransaction07Criteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Batch")]
        public int TranSize { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_FromTransaction07";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
