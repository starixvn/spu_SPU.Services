using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class CategorizeCustomerRealtimeCriteria : CriteriaBase
    {
        public string VIPCustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CustomerClassification_CategorizeCustomer_RealTime";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(VIPCustomersXML);
        }
    }
}
