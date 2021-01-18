namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCustomersFromMainDbCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "LastCustId")]
        public Int64 LastCustId { get; set; }

        [SpParam(SqlParamName = "rowSize")]
        public int RowSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_GetNewCustomerInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
