using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria
{
    public class TransformAssociationAffectedEvidenceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Batch")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_TransformAssociation_AffectedEvidence";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
