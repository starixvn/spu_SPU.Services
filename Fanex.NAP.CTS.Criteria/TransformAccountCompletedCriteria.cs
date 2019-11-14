using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
   public class TransformAccountCompletedCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AccountJson")]
        public string AccountJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCST_Account_Completed";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
