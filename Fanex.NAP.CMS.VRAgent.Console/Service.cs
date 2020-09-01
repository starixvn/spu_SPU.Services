namespace Fanex.NAP.CMS.VRAgent.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
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
            LogService.Info("Start executing for VR - Auto Agent Adjustment service.");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"VR - Auto Agent Adjustment service ({AppSettings.EnvironmentName}) is running");
            }

            return this;
        }

        public override BaseService UpdateVRAgent()
        {
            var service = new VRAgentService(AppSettings.SiteIds, AppSettings.SuperIds, DateTime.Now);
            var countRetry = 0;

            while (countRetry < AppSettings.RetryTime)
            {
                try
                {
                    service.AutoVRAgentAdjustment();
                    countRetry = AppSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Auto Agent Adjustment service - Start Exception {logMessage}");
                    countRetry += 1;
                    if (countRetry == AppSettings.RetryTime)
                    {
                        throw ex;
                    }
                }
            }

            TotalAffectedMembers = service.PuntersUpdated;

            LogService.Info("Executed for VR - Auto Agent Adjustment successfully!");

            _stopTime = DateTime.Now;

            return this;
        }

        public override BaseService NotifyMessage()
        {
            var environment = AppSettings.EnvironmentName;

            LogService.Info($"VR - Auto Agent Adjustment service ({ environment }) - TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");

            if (AppSettings.Environment == 3)
            {
                _letstalkService.Info($"VR - Auto Agent Adjustment service run complete ({ environment }). TotalAffectedMembers: {TotalAffectedMembers} at start time { _startTime }");
            }

            return this;
        }
    }
}
