using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Criteria
{
    public class GetRawUnsucessAssociationListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfAssociation")]
        public int NumberOfAssociation { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCTS_Association_Unsuccess_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
