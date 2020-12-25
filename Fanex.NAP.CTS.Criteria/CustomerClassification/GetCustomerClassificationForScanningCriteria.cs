namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerClassificationForScanningCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int RecordQuantityPerBatch { get; set; }

        [SpParam(SqlParamName = "ip_Batch")]
        public int BatchQuantity { get; set; }
        
        public override string GetSettingKey()
        {
            return "CTS_DC_GetCustomerClassificationIntervalScan";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
