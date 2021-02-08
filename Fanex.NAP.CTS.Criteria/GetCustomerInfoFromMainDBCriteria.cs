namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerInfoFromMainDBCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "custIdList")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "sourceType")]
        public int SourceType { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_GetInfoByCustIdList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
