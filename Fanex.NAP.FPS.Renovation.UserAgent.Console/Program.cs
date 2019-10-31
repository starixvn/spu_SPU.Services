namespace Fanex.NAP.FPS.Renovation.UserAgent.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    static class Program
    {
        static void Main(string[] args)
        {
            var userAgent = new Services.Implement.UserAgent();

            try
            {
                var drawUserAgent = userAgent.GetUserAgent(Core.AppSettings.SizeOfUserAgent);

                if (drawUserAgent.Any())
                {                   
                    var userAgentJson = JsonConvert.SerializeObject(drawUserAgent);

                    userAgent.UpdateUserAgent(userAgentJson);
                }
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
