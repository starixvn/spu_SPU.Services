namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using System.Collections.Generic;

    public interface IPunterService
    {
        IPunterService GetWinningPunterSoccerList();

        IPunterService GetWinningPunterList();

        IPunterService FilterPunterBaseOnCurrentVirtualRate(short sportType, bool compareVROtherSports);

        IPunterService AdjustVirtualSettingForSuperProPunter(double virtualMultiplier);

        IPunterService AdjustVirtualSettingForNormal(double virtualMultiplier);

        IEnumerable<int> GetVRAdjustmentCustomers();

        VerifiedVRCustomers VerifyVRAdjustmentCustomers(string customerList);

        IPunterService UpdateVRFollowUpline(string customerList);

        IPunterService UpdateVRAutoAdjustmentStatus(string customerList);

        int CountAffected();
    }
}