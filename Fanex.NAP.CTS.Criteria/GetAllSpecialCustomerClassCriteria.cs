namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAllSpecialCustomerClassCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsGetAllInfo")]
        public bool IsGetAllInfo { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetAllSpecialCustomerClass";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
