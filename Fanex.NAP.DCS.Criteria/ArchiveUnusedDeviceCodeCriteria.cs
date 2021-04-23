namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ArchiveUnusedDeviceCodeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "op_IsContinue")]
        public bool IsContinue { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_DeviceCode";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
