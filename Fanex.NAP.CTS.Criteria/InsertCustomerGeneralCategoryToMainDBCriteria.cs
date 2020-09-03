namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data.Repository;

    public class InsertCustomerGeneralCategoryToMainDBCriteria : NonQueryCommand
    {
        public string custList { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdateCustomerClass";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
