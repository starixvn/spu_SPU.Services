namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetProbationCustForScanningCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int NumberOfRecord { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Probation_GetCustForScanning";
        }

        public override bool IsValid()
        {
            return NumberOfRecord > 0;
        }
    }
}
