namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;

    public class CustomerClassificationUpdateNewClassificationCriteria : NonQueryCommand
    {
        public string ListCustIds { get; set; }

        public bool IsGeneralCategory { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_UpdateAppliedStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
