namespace Fanex.NAP.CTS.Criteria.MainDB
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UpdateBetLimitByEvidenceRemovingCriteria : NonQueryCommand
    {
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_BLControl_EvidenceFlagged";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
