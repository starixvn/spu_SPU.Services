using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class InsertAssociationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AssociationList")]
        public string AssociationJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_InsertAssociation";
        }

        public override bool IsValid()
        {
            return true; 
        }
    }
}
