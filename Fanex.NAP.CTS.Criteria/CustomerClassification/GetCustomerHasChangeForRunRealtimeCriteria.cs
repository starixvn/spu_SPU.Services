
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class GetCustomerHasChangeForRunRealtimeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_CustomerClassification_CategorizeCustomer_RealTime_CheckChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
