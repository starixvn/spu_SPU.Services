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
        public string EvidenceId { get; set; }
        public override string GetSettingKey() => "CTS_DC_GetCustEvidenceByEvidenceID";

        public override bool IsValid() => !string.IsNullOrEmpty(EvidenceId);
    }
}
