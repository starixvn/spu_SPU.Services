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
        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ExecuteJobs();

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
            var msg = $"Executing service is SHUTDOWN at {DateTime.Now}";

            LogService.Info(msg);

            if (AppSetting.Environment == 3)
            {
                new LetstalkService().Info(msg);
            }

            base.OnShutdown();
        }

        private void ExecuteJobs()
        {
            var config = (NameValueCollection)ConfigurationManager.GetSection("quartz");

            ISchedulerFactory factory = new StdSchedulerFactory(config);

            var schedule = factory.GetScheduler().Result;

            if (!schedule.IsStarted)
            {
                schedule.Start().Wait();

                //Task.Delay(TimeSpan.FromMinutes(1)).Wait();
            }            
        }        
    }
}