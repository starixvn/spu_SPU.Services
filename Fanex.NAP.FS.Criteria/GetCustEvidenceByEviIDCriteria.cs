namespace Fanex.NAP.FS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetCustEvidenceByEviIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_EvidenceID")]
        public int EvidenceId { get; set; }
        public override string GetSettingKey() => "CTS_DC_GetCustEvidenceByEvidenceID";

        public override bool IsValid() => EvidenceId > 0 ? true : false;
    }
}
