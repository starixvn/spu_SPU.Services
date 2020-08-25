namespace Fanex.NAP.CMS.BetLimit.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.MailDelivery.Client;
    using Fanex.NAP.CMS.VirtualSetting.Service;
    using Fanex.NAP.SPU.Core;
    using System;

    public class Service : BaseService
    {
        private DateTime _startTime;

        private DateTime _stopTime;

        public Service StartTime()
        {
            _startTime = DateTime.Now;
            LogService.Info("Start executing for Auto Update Bet Limit.");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"Auto Update Bet Limit service ({AppSettings.EnvironmentName}) is running");
            }
            
            return this;
        }

        public override BaseService UpdateBetLimit()
        {
            var service = new BetLimitService(AppSettings.SiteIds, AppSettings.SuperIds, DateTime.Now);
            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    //service.AutoAdjustBetLimit();
                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"Auto Update Bet Limit service - Start Exception {logMessage}");
                    countRetry += 1;
                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw ex;
                    }
                }
            }

            TotalAffectedMembers = service.PuntersUpdated;

            LogService.Info("Executed for Auto Update Bet Limit successfully!");

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService NotifyMessage()
        {
            var environment = AppSettings.EnvironmentName;

            LogService.Info($"Auto Update Bet Limit service ({ environment }) - SendMailExternal successful TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"Auto Update Bet Limit service run complete ({ environment }). TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");
            }

            return this;
        }             
    }
}