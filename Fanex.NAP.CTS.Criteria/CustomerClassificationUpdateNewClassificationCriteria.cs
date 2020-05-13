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
        public long MaxCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CustomerClassification_UpdateNewClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
