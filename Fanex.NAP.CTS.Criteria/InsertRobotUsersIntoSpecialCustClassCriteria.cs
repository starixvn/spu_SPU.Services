namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertRobotUsersIntoSpecialCustClassCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialCustClass_InsertRobotUsers";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIds);
        }
    }
}
