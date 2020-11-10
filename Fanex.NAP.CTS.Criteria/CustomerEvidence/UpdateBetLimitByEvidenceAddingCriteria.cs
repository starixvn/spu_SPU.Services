namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateBetLimitByEvidenceAddingCriteria : CriteriaBase
    {
        public string CustIDList { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "NAP_BLControl_EvidenceFlagged";

        public override bool IsValid()
            => true;
    }
}
