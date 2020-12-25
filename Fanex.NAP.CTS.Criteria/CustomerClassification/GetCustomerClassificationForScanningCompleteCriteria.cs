namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerClassificationForScanningCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCustomerClassificationIntervalScan_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
