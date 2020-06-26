using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class CategorizeCustomerDailyCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }

        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CustomerClassification_CategorizeCustomer_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
