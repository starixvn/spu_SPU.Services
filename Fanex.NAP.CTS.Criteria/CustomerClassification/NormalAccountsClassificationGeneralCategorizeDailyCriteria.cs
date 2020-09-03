using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    public class NormalAccountsClassificationGeneralCategorizeDailyCriteria : NonQueryCommand
    {
        public string ListCustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GeneralCategorize_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
