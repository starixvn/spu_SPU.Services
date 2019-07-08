namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    public interface IPunterService
    {
        IPunterService GetWinningPunterSoccerList();

        IPunterService GetWinningPunterList();

        IPunterService FilterPunterBaseOnCurrentVirtualRate(short sportType, bool compareVROtherSports);

        IPunterService AdjustVirtualSettingForSuperProPunter(double virtualMultiplier);

        IPunterService AdjustVirtualSettingForNormal(double virtualMultiplier);

        int CountAffected();
    }
}