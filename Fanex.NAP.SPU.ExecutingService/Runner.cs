namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Service;
    using Quartz;
    using Quartz.Impl;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.ServiceProcess;
    using System.Threading.Tasks;

    public partial class Runner : ServiceBase
    {
        IScheduler scheduler;

        public Runner()
        {
            InitializeComponent();

            var config = (NameValueCollection)ConfigurationManager.GetSection("quartz");

            ISchedulerFactory factory = new StdSchedulerFactory(config);

            scheduler = factory.GetScheduler().Result;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (!scheduler.IsStarted)
                {
                    scheduler.Start().Wait();
                }

                var msg = $"Executing service is STARTED at {DateTime.Now}";

                LogService.Info(msg);

                if (AppSetting.Environment == 3)
                {
                    new LetstalkService().Info(msg);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"Executing service is START error message: { ex.Message } at {DateTime.Now}";

                LogService.Error(errorMsg);

                if (AppSetting.Environment == 3)
                {
                    new LetstalkService().Error(errorMsg);
                }
            }
        }

        protected override void OnStop()
        {
            if(scheduler.IsStarted)
            {
                scheduler.Shutdown(false);
            }

            var msg = $"Executing service is STOP at {DateTime.Now}";

            LogService.Info(msg);

            if (AppSetting.Environment == 3)
            {
                new LetstalkService().Info(msg);
            }

            base.OnStop();
        }

        protected override void OnShutdown()
        {
            scheduler.Shutdown(false);

            var msg = $"Executing service is SHUTDOWN at {DateTime.Now}";

            LogService.Info(msg);

            if (AppSetting.Environment == 3)
            {
                new LetstalkService().Info(msg);
            }

            base.OnShutdown();
        }        
    }
}