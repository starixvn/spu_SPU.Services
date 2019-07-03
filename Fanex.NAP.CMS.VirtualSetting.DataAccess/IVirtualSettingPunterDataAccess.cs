namespace Fanex.NAP.CMS.VirtualSetting.DataAccess
{
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.Framework.Command;
    using System;
    using System.Collections.Generic;

    interface IVirtualSettingPunterDataAccess
    {      
        ICommandResult UpdateVirtualSetting(string VirtualMultiplier, int userId, string ip, short sportType);

        ICommandResult LogVirtualSetting(string punterVRSettingsDoc, int creatorId, string serviceName, DateTime adjustDate);

        IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds);

        ICommandResult InsertCustomerMarginHistory(string customerMarginDoc, DateTime adjustDate);

        IEnumerable<CustomerMargin> GetCustomerMarginHistory(string custIds, byte sportGroupId, int numOfMonth);

        IEnumerable<int> GetBACustIds(string custIds);


    }
}
