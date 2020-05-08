namespace Fanex.NAP.TGRS.AutoTradingViaEmail.Service
{
    using Fanex.MailDelivery.Client;
    using System.ServiceProcess;

    partial class Runner : ServiceBase
    {
        public Runner()
        {
            InitializeComponent();           
        }

        protected override void OnStart(string[] args)
        {
            MailClientHandler.Start();
        }

        protected override void OnStop()
        {
            MailClientHandler.Stop();
        }
    }
}