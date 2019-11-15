using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
   public class TransformAssociationCompletedCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AssociationJson")]
        public string AssociationJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCST_Association_Completed";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
