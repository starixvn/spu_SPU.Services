using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class GetCustomerClassificationForScanningCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int RecordQuantityPerBatch { get; set; }

        [SpParam(SqlParamName = "ip_Batch")]
        public int BatchQuantity { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCustomerClassificationForScanning";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
