namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;

    public class GetUsername2FromMainDb : CriteriaBase
    {
        public string CustIds { get; set; }

        public bool IsGetCustSub { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GetMainDbCustomerForUpdateUsername2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
