namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using System.Collections.Generic;

    public interface IPunterService
    {
        IEnumerable<Punter> Punters { get; set; }

        IPunterService GetPunterList();

        IPunterService FilterPunterBaseOnCurrentVirtualRate();

        IPunterService AdjustVirtualSettingForSuperProPunter(double virtualMultiplier);

        IPunterService AdjustVirtualSettingForNormal(double virtualMultiplier);

    }
}