namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using System.Collections.Generic;

    public interface IVirtualSettingService
    {
        IEnumerable<Punter> Punters { get; set; }

        IVirtualSettingService AutoAdjustVirtualSettingsForPunters();
    }
}