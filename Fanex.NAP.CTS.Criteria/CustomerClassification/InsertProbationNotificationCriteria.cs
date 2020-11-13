namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class InsertProbationNotificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TotalScanned")]
        public int TotalScanned { get; set; }

        [SpParam(SqlParamName = "ip_TotalFailed")]
        public int TotalFailed { get; set; }

        [SpParam(SqlParamName = "ip_TotalGeneralFailed")]
        public int TotalGeneralFailed { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Notification_InsertProbationNotification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
