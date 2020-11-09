namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;

    public class GetCustomerEvidenceRawCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceRaw_Get";

        public override bool IsValid()
            => true;

    }
}
