using System.Collections.Generic;

namespace Fanex.NAP.CMS.VirtualSetting.Model
{
    public class VRGetMemberBigStakePerformanceModel
    {
        public IEnumerable<VRCheckMemberBigStakePerformanceFirstDayModel> FirstBetDateList { get; set; }

        public IEnumerable<VRCheckMemberBigStakePerformanceStatusModel> StatusList { get; set; }
    }
}
