namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class GetCustomersWithHavingUsername2Criteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custXML")]
        public string cust { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_GetNotNULLUserName2ByXML";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
