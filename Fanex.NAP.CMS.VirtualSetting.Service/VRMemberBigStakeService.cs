using Fanex.NAP.CMS.VirtualSetting.DataAccess;
using Fanex.NAP.CMS.VirtualSetting.Model;
using System;
using System.Collections.Generic;

namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    public class VRMemberBigStakeService
    {
        private readonly IVirtualSettingDataAccess virtualSettingDataAccess;

        public VRMemberBigStakeService()
        {
            virtualSettingDataAccess = new VirtualSettingDataAccess();
        }

        public void InsertVRMemberBigStake()
        {
            virtualSettingDataAccess.InsertVRMemberBigStake();
        }

        public VRGetMemberBigStakePerformanceModel GetMemberBigStakePerformance()
        {
            return virtualSettingDataAccess.GetMemberBigStakePerformance();
        }

        public void UpdateVRMemeberBigStakeFirstBetDate(IEnumerable<long> customerList, int type, DateTime betDate)
        {
            var custIdList = string.Join(",", customerList);

            virtualSettingDataAccess.UpdateVRMemeberBigStakeFirstBetDate(custIdList, type, betDate);
        }

        public void UpdateVRWhitelistMemberBigStake(IEnumerable<long> customerList, int type)
        {
            var custIdList = string.Join(",", customerList);

            virtualSettingDataAccess.UpdateVRWhitelistMemberBigStake(custIdList, type);
        }

        public VRAutomainBigCreditMemberViewModel GetBigCreditMemberForAutomail()
        {
            return virtualSettingDataAccess.GetBigCreditMemberForAutomail();
        }
    }
}
