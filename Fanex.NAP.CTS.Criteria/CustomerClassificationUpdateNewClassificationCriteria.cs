using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class CustomerClassificationUpdateNewClassificationCriteria : NonQueryCommand
    {
        public string ListCustIds { get; set; }

        public bool IsGeneralCategory { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_UpdateAppliedStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
