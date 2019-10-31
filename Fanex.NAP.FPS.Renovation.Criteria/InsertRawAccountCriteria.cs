using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Criteria
{
    public class InsertRawAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AccountJson")]
        public string AccountJson { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_TransformData_Account_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
