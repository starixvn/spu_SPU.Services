namespace Fanex.NAP.FS.DailyDataCapture.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.MailDeliver.Model;
    using Fanex.NAP.FS.Service;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Configuration;

    public class Service : BaseService
    {
        private readonly FS.Service.DailyDataCaptureService _service;

        public Service() : base(Model.FraudCaseId.DailyDataCapture)
        {
            _service = new FS.Service.DailyDataCaptureService();
        }

        public override BaseService Rule()
        {
            Execute();

            return base.Rule();
        }

        private void Execute()
        {
            try
            {
                _service.Rule();

                SendMailInternal();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Daily Data Capture service - Execute Exeption {logMessage}");
            }
        }

        public void SendMailInternal()
        {
            var environment = AppSettings.EnvironmentName;

            using (var client = new Fanex.MailDeliver.MailClient.API.MailDeliver())
            {
                var mailItem = new MailRecord
                {
                    MailTo = "spu@starixsoft.com",
                    Subject = "Daily Data Capturing Service" + " (" + environment + ")",
                    Title = "Daily Data Capturing Service - Still alive",
                    Body = $"Please be informed that Daily Data Capturing Service is EXECUTED successfully at {DateTime.Now} (GMT-4).",
                    Priority = System.Net.Mail.MailPriority.High
                };

                client.SendMail(mailItem);
            }
        }
    }
}