namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCustSubFromMainDbCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "LastCustSubId")]
        public Int64 LastCustSubId { get; set; }

        [SpParam(SqlParamName = "rowSize")]
        public int RowSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_GetNewCustSubInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
