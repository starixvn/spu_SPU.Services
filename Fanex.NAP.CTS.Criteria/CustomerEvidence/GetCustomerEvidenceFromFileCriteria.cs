namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;

    public class GetCustomerEvidenceFromFileCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceFromFile_Get";

        public override bool IsValid()
            => true;

    }
}
