namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UpdateLastScanDateForProbationCustCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Probation_UpdateLastScanDate";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustSportJson);
        }
    }
}
