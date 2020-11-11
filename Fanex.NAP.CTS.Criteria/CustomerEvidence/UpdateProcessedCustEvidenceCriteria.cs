namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateProcessedCustEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IDList")]
        public string IdList { get; set; }

        [SpParam(SqlParamName = "ip_UpdatedBy")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceFromFile_UpdateProcessed";

        public override bool IsValid()
            => true;
    }

}
