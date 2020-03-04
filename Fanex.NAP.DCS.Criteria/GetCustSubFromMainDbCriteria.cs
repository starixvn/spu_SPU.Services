namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data.Repository;

    public class GetCustSubFromMainDbCriteria : CriteriaBase
    {
        public int MaxCustId { get; set; }

        public int RowSize { get; set; }

        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GetCustSubInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
