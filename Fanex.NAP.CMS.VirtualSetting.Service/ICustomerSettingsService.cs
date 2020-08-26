namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    public interface ICustomerSettingsService
    {
        ICustomerSettingsService AutoAdjustVirtualSettings();

        ICustomerSettingsService AutoAdjustBetLimit();
    }
}