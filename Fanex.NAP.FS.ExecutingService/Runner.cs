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
        private List<JobDataSevices> listJobServicesMap;

        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                listJobServicesMap = GetJobServicesMap();

                if (!listJobServicesMap.Any())
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
                LogService.Info($"Executing service - GetDataMap error message: { ex.ToString() } at {DateTime.Now}");
            }

            return new List<JobDataSevices>();
        }

        private void ExecuteJobs()
        {
            foreach (var serviceMap in listJobServicesMap)
            {
                if (!serviceMap.Active)
                {
                    continue;
                }

                if (serviceMap.Id == 1)
                {
                    CreateJobByFraudCase(serviceMap);

                    continue;
                }

                CreateJob(serviceMap);
            }
        }

        private void CreateJobByFraudCase(JobDataSevices servicesMap)
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

                var job = new JobBase();
                var name = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);
                var className = string.Format("{0}Job", name);
                var jobName = className;
                var triggerName = string.Format("{0}Trigger", name);
                var mapData = GetJobData(fraudCase.FraudCaseId, servicesMap);

                job.CreateJob(className, jobName, GroupName, mapData)
                    .CreateTrigger(triggerName, fraudCase.FraudCaseInterval)
                    .CreateSchedule()
                    .CreateListenerJob()
                    .Run();
            }
            
        }

        private void CreateJob(JobDataSevices servicesMap)
        {
            foreach (var jobData in servicesMap.Jobs)
            { 
                var job = new JobBase();                
                var className = string.Format("{0}Job", jobData.Name);
                var jobName = className;
                var triggerName = string.Format("{0}Trigger", servicesMap.Name);                

                job.CreateJob(className, jobName, GroupName, jobData)
                    .CreateTrigger(triggerName, 60)
                    .CreateSchedule()
                    .CreateListenerJob()
                    .Run();
            }
        }


        private JobData GetJobData(int jobId, JobDataSevices servicesMap)
        {
            var jobData = servicesMap.Jobs.Find(c => c.Id == jobId);

            if (jobData == null)
            {
                return new JobData();
            }

            if (!servicesMap.Active)
            {
                jobData.Active = false;
            }

            return jobData;
        }
    }
}