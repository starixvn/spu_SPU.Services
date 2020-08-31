namespace Fanex.NAP.CMS.VirtualSetting.DataAccess
{
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.Framework.Command;
    using System;
    using System.Collections.Generic;

    public interface IVirtualSettingDataAccess
    {
        IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds);

        IEnumerable<int> GetBACustIds(string custIds);

        IEnumerable<Punter> GetWinningPunterSoccerList(string siteNames, string superIds, double virtualRateThreshold);

        IEnumerable<Punter> GetWinningPunterList(string siteNames, string superIds, double virtualRateThreshold);

        ICommandResult UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip, short sportType);

        ICommandResult LogPunterVirtualSetting(string serviceName, IEnumerable<Punter> punters, int userId, DateTime adjustDate);

        ICommandResult InsertCustomerMarginHistory(IEnumerable<Punter> punters, byte sportGroupID, int numOfMonth, string serviceName, DateTime adjustDate);

        IEnumerable<CustomerMargin> GetCustomerMarginHistory(IEnumerable<Punter> punters, byte sportGroupId, int numOfMonth);

        #region OtherSports VR
        IEnumerable<int> GetVRAdjustmentCustomers();

        (IEnumerable<int>, IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList);

        void UpdateVRFollowUpline(string customerList);

        void UpdateVRAutoAdjustmentStatus(string customerList);
        #endregion

        #region Bet Limit
        IEnumerable<int> GetBetLimitCustomers();

        (IEnumerable<int>, IEnumerable<int>) VerifyBetLimitCustomers(string customerList);

        void UpdateBetLimit(string customerList);

        void RemoveBetLimit(string customerList);
        #endregion


        #region VR-Agent
        void InsertNewVRAgent();

        (IEnumerable<int>, IEnumerable<int>) VerifySportsAgentPerformance();

        void UpdateFlagAbnormalAgent(string customerList, int type);

        void UpdateUnflagAbnormalAgent(string customerList, int type);
        #endregion
    }
}