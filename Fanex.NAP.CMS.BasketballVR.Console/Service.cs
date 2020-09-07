namespace Fanex.NAP.CMS.BasketballVR.Console
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
            LogService.Info("Start executing for VR-Basketball.");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"Basketball - Virtual Setting service ({AppSettings.EnvironmentName}) is running");
            }
            
            return this;
        }

        public override BaseService UpdateVRForBasketball()
        {
            VROthersportsService service = new VROthersportsService(AppSettings.SiteIds, AppSettings.SuperIds, DateTime.Now);
            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    service.AutoAdjustVirtualSettings();
                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"Basketball - VirtualSetting service - Start Exception {logMessage}");
                    countRetry += 1;
                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw ex;
                    }
                }
            }

            TotalAffectedMembers = service.PuntersUpdated;

            LogService.Info("Execute For Basketball is called successfully.");

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService UpdateVRForSpecialSportBooks()
        {
            VROthersportsService service = new VROthersportsService(AppSettings.SiteIds, AppSettings.SuperIds, DateTime.Now);
            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    service.AutoAdjustVRForSpecialSportBook();
                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"Sports Book - VirtualSetting service - Start Exception {logMessage}");
                    countRetry += 1;
                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw ex;
                    }
                }
            }

            TotalAffectedForSpecicalSport = service.PuntersUpdated;

            LogService.Info("Execute For SportsBook is called successfully.");

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService NotifyMessage()
        {
            var environment = AppSettings.EnvironmentName;

            LogService.Info($"Basketball- Virtual Setting service ({ environment }) - SendMailExternal successful TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");

            LogService.Info($"Special Sport - Virtual Setting service ({ environment }) - SendMailExternal successful TotalAffectedMembers: {TotalAffectedForSpecicalSport} at start time { _startTime }");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"Basketball - Virtual Setting service run complete ({ environment }). TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");
                _letstalkService.Info($"Special Sport - Virtual Setting service run complete ({ environment }). TotalAffectedMembers: {TotalAffectedForSpecicalSport} at start time { _startTime }");
            }

            return this;
        }             
    }
}