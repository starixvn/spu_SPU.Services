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
