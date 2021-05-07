using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Criteria
{
    public class ProcessTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromTransID")]
        public long FromStagingTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToTransID")]
        public long ToStagingTransId { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformData_Transaction_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
