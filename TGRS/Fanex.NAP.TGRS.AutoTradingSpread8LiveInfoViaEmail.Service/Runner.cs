namespace Fanex.NAP.TGRS.AutoTradingSpread8LiveInfoViaEmail.Service
{
    using Fanex.MailDelivery.Client;
    using Fanex.NAP.SPU.Core;
    using Fanex.Logging;
    using System;
    using System.ServiceProcess;

    partial class Runner : ServiceBase
    {
        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                MailClientHandler.Start();
            }
            catch (Exception ex)
            {
                LogService.Warn($"TGRS - Fanex.NAP.TGRS.AutoTradingSpread8LiveInfoViaEmail.Service ({AppSettings.EnvironmentName}) OnStart fail: {ex.ToString()}");
            }
        }

        protected override void OnStop()
        {
            try
            {
                MailClientHandler.Stop();
            }
            catch (Exception ex)
            {
                LogService.Warn($"TGRS - Fanex.NAP.TGRS.AutoTradingSpread8LiveInfoViaEmail.Service ({AppSettings.EnvironmentName}) OnStop fail: {ex.ToString()}");
            }
        }
    }
}
