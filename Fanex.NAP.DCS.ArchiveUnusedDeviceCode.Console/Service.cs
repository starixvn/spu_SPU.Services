namespace Fanex.NAP.DCS.ArchiveUnusedDeviceCode.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.DCS.Services.Implement;
    using Fanex.NAP.SPU.Core;
    using Newtonsoft.Json;
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
                bool isWriteLog = AppSettings.IsWriteLogInfo;
                var isContinue = false;

                do
                {
                    isContinue = _dataCenterService.ArchiveUnusedDeviceCode(AppSettings.InternalChunkSize);

                    
                     LogService.Info($"ArchiveUnusedDeviceCode.Service - IsContinue: {JsonConvert.SerializeObject(isContinue)} - Time: {DateTime.Now}");

                } while (isContinue);

                Thread.Sleep(30000);
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
