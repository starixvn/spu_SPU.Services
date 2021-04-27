namespace Fanex.NAP.DCS.ArchiveUnusedDeviceCode.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Threading;

    public class Service
    {
        private readonly DataCenterService _dataCenterService;

        public Service()
        {
            _dataCenterService = new DataCenterService();
        }

        public void Process()
        {
            try
            {
                bool isWriteLogInfo = AppSettings.IsWriteLogInfo;
                var isContinue = true;

                while (isContinue)
                {
                    isContinue = _dataCenterService.ArchiveUnusedDeviceCode(AppSettings.InternalChunkSize);

                    if (isWriteLogInfo)
                        LogService.Info($"ArchiveUnusedDeviceCode.Service - IsContinue: {isContinue} - Time: {DateTime.Now}");

                    Thread.Sleep(AppSettings.BreakTime);
                };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }
    }
}
