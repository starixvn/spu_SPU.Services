namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data.Repository;
    
    public class ClassifyCategoryForProbationCustCriteria : CriteriaBase
    {
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_Probation";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomersXML);
        }
    }
}
