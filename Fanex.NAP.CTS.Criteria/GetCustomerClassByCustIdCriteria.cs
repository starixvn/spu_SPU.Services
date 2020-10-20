namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerClassByCustIdCriteria : NonQueryCommand
    {
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerClass_GetByCustId";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}