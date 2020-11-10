namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateProcessCustEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IDList")]
        public string IdList { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "";

        public override bool IsValid()
            => true;
    }

}
