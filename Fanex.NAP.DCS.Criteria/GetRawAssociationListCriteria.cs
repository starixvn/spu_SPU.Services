using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.DCS.Criteria
{
    public class GetRawAssociationListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfTransaction")]
        public int NumberOfAssociation { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCTS_Association_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
