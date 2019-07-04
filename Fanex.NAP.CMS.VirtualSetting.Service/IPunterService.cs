namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    public interface IPunterService
    {
        IPunterService GetPunterList();

        IPunterService FilterPunterBaseOnCurrentVirtualRate();

        IPunterService AdjustVirtualSettingForSuperProPunter(double virtualMultiplier);

        IPunterService AdjustVirtualSettingForNormal(double virtualMultiplier);

    }
}