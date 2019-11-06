namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.ExecutingService.Jobs;
    using Fanex.NAP.SPU.Model;
    using Fanex.NAP.SPU.Service;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;

    public partial class Runner : ServiceBase
    {
        private FraudSensorJob fraudSensorJob;
        private VirtualSettingJob virtualSettingJob;        
        private FPSJob fpsJob;
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
            var listJobServicesMap = GetJobServicesMap();

            if (!listJobServicesMap.Any())
            {
                LogService.Error($"Executing service is error message: JobsMap null");

                return;
            }

            var jobBase = new JobBase();

            foreach (var serviceMap in listJobServicesMap)
            {
                switch (serviceMap.Id)
                {
                    case 1:
                        {
                            fraudSensorJob = new FraudSensorJob(jobBase);

                            fraudSensorJob.CreateScheduleJob<ExecuteConsoleJob>(serviceMap);

                            break;
                        }
                    case 2:
                        {
                            virtualSettingJob = new VirtualSettingJob(jobBase);

                            virtualSettingJob.CreateScheduleJob<ExecuteConsoleJob>(serviceMap);

                            break;
                        }
                    case 4:
                        {
                            fpsJob = new FPSJob(jobBase);

                            fpsJob.CreateScheduleJob<ExecuteConsoleJob>(serviceMap);

                            break;
                        }
                    case 5:
                        {
                            fpsJob = new FPSJob(jobBase);

                            fpsJob.CreateScheduleJob<ExecuteTransactionAPIJob>(serviceMap);

                            break;
                        }
                    case 6:
                        {
                            fpsJob = new FPSJob(jobBase);

                            fpsJob.CreateScheduleJob<ExecuteDeviceAPIJob>(serviceMap);

                            break;
                        }
                }
            }
        }

        private List<JobDataSevices> GetJobServicesMap()
        {
            try
            {
                var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + @"App_Data\JobData.xml";

                if (File.Exists(xmlPath))
                {
                    var rootMap = Serializer.Deserialize<JobDataRoot>(xmlPath);

                    if (rootMap != null)
                    {
                        return rootMap.Sevices;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"Executing service - GetDataMap error message: { ex.Message } at {DateTime.Now}");
            }

            return new List<JobDataSevices>();
        }
    }
}