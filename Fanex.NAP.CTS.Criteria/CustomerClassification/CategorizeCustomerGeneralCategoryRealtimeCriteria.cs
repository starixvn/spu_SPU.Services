using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class CategorizeCustomerGeneralCategoryRealtimeCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GeneralCategorize_RealTime";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomersXML);
        }
    }
}
