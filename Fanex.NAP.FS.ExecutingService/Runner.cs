namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.Logging;
    using System;
    using System.ServiceProcess;    
    using Fanex.NAP.FS.Service;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Model;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Runner : ServiceBase
    {
        private const string GroupName = "FraudCaseGroup";
        private List<JobDataSevices> listJobsMap;

        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                listJobsMap = GetJobsMap();

                if (!listJobsMap.Any())
                {
                    LogService.Info($"Executing service is error message: JobsMap null");

                    return;
                }

                ExecuteJobs();

                LogService.Info($"Executing service is STARTED");
            }
            catch (Exception ex)
            {
                LogService.Error($"Executing service is START error message: { ex.ToString() } at {DateTime.Now}");
            }
        }

        protected override void OnStop()
        {
            LogService.Info($"Executing service is STOP at {DateTime.Now}");
        }

        private void ExecuteJobs()
        {
            var fraudCaseService = new FraudCaseService();

            var fraudCases = fraudCaseService.GetFraudCase<FS.Model.FraudCase>(null, 1, AppSetting.Environment);

            foreach (var fraudCase in fraudCases)
            {
                if (!fraudCase.IsActive)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId)))
                {
                    continue;
                }
                
                var job = new JobFS();
                var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);
                var className = string.Format("{0}Job", fraudCaseName);
                var jobName = className;
                var triggerName = string.Format("{0}Trigger", fraudCaseName);
                var mapping = GetJobData(fraudCase.FraudCaseId);

                job.CreateJob(className, jobName, GroupName, mapping)
                    .CreateTrigger(triggerName, fraudCase.FraudCaseInterval)
                    .CreateSchedule()
                    .CreateListenerJob()
                    .Run();
            }
        }

        private List<JobDataSevices> GetJobsMap()
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
                LogService.Info($"Executing service - GetDataMap error message: { ex.ToString() } at {DateTime.Now}");
            }

            return new List<JobDataSevices>();
        }

        private JobData GetJobData(int id)
        {
            return new JobData();
        }
    }
}