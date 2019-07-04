namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;

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

                LogService.Info($"Executing service is STARTED");
            }
            catch (Exception ex)
            {
                LogService.Error($"Executing service is START error message: { ex.Message } at {DateTime.Now}");
            }
        }

        protected override void OnStop()
        {
            LogService.Info($"Executing service is STOP at {DateTime.Now}");
        }

        private void ExecuteJobs()
        {
            var listJobServicesMap = GetJobServicesMap();

            if (!listJobServicesMap.Any())
            {
                LogService.Error($"Executing service is error message: JobsMap null");

                return;
            }

            foreach (var serviceMap in listJobServicesMap)
            {
                if (serviceMap.Active)
                {
                    switch (serviceMap.Id)
                    {
                        case 1:
                            {
                                var fraudSensorJob = new FraudSensorJob();

                                fraudSensorJob.CreateScheduleJob(serviceMap);

                                break;
                            }
                        default:
                            {
                                var virtualSettingJob = new VirtualSettingJob();

                                virtualSettingJob.CreateScheduleJob(serviceMap);

                                break;
                            }
                    }
                }
            }
        }

        private List<JobDataSevices> GetJobServicesMap()
        {
            try
            {
                var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + @"App_Data\JobData.xml";
                var rootMap = Serializer.Deserialize<JobDataRoot>(xmlPath);

                if (rootMap != null)
                {
                    return rootMap.Sevices;
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