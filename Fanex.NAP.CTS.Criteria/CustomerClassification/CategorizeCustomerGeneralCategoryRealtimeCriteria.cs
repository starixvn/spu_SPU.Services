namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data.Repository;

    public class CategorizeCustomerGeneralCategoryRealtimeCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GeneralCategorize_RealTime";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomersXML);
        }
    }
}
