namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data.Repository;

    public class NormalAccountsClassificationGeneralCategorizeDailyCriteria : NonQueryCommand
    {
        public string ListCustIds { get; set; }

        public bool IsCheckLast14Days { get; set; }

        public int FunctionId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GeneralCategorize_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
