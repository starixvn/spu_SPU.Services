namespace Fanex.NAP.FPS.Renovation.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    public class GetNewCustomerCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_TransformData_Account_GetForUpdateCustInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}   
