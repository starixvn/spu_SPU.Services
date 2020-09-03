namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data.Repository;

    public class GetCustomerGeneralCategoryHasChangeForRunRealtimeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GeneralCategorize_RealTime_CheckChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
