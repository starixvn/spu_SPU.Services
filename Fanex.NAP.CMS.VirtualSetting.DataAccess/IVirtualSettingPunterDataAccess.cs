namespace Fanex.NAP.CMS.VirtualSetting.DataAccess
{
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.Framework.Command;
    using System;
    using System.Collections.Generic;

    public interface IVirtualSettingPunterDataAccess
    {
        ICommandResult UpdateVirtualSetting(string VirtualMultiplier, int userId, string ip, short sportType);

        ICommandResult LogVirtualSetting(string punterVRSettingsDoc, int creatorId, string serviceName, DateTime adjustDate);

        IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds);

        IEnumerable<int> GetBACustIds(string custIds);

        IEnumerable<Punter> GetWinningPunterSoccerList(string siteNames, string superIds, double virtualRateThreshold);

        ICommandResult UpdatePunterVirtualSettingForSoccer(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip);

        ICommandResult LogPunterVirtualSetting(string serviceName, IEnumerable<Punter> punters, int userId, DateTime adjustDate);

        ICommandResult InsertCustomerMarginHistory(IEnumerable<Punter> punters, byte sportGroupID, int numOfMonth, string serviceName, DateTime adjustDate);

        IEnumerable<CustomerMargin> GetCustomerMarginHistory(IEnumerable<Punter> punters, byte sportGroupId, int numOfMonth);
    }
}