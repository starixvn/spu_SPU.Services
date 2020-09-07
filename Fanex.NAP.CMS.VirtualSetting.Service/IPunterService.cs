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

        (IEnumerable<int>, IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList);

        IPunterService UpdateVRFollowUpline(string customerList);

        IPunterService UpdateVRAutoAdjustmentStatus(string customerList);

        IEnumerable<int> GetBetLimitCustomers();

        (IEnumerable<int>, IEnumerable<int>) VerifyBetLimitCustomers(string customerList);

        IPunterService UpdateBetLimit(string customerList);

        IPunterService RemoveBetLimit(string customerList);

        int CountAffected();

        IPunterService InsertNewVRAgent();

        (IEnumerable<int>, IEnumerable<int>) VerifySportsAgentPerformance();

        IPunterService UpdateFlagAbnormalAgent(string customerList, int type);

        IPunterService UpdateUnflagAbnormalAgent(string customerList, int type);
    }
}