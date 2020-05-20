using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class AutoTagCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NewClassification")]
        public string NewClassification { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AutoTagCustomerClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
