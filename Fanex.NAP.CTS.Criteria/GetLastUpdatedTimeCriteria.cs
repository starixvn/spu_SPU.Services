namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;

    public class GetLastUpdatedTimeCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_GetLastUpdateTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}