namespace Fanex.NAP.SPU.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.Service;
    using Fanex.NAP.SPU.Model;
    using System;

    public class FraudSensorJob : ScheduleJob
    {
        private const string groupName = "FraudSensor";

        private readonly JobBase _job;

        public FraudSensorJob(JobBase job)
        {
            _job = job ?? new JobBase();

            _job.GetSchedule();
        }

        public override void CreateScheduleJob<T>(JobDataSevices jobData)
        {
            var fraudCaseService = new FraudCaseService();
            var fraudCases = fraudCaseService.GetFraudCase<FS.Model.FraudCase>(null, 1, AppSetting.Environment);

            foreach (var fraudCase in fraudCases)
            {                
                var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);
                var jobName = string.Format("{0}Job", fraudCaseName);
                var triggerName = string.Format("{0}Trigger", fraudCaseName);
                var mapData = GetJobData(fraudCase.FraudCaseId, jobData);

                _job.GroupName = groupName;
                _job.TriggerName = triggerName;
                _job.JobName = jobName;

                _job.CreateJob<T>(mapData)
                    .CreateTriggerIntervalInSeconds(fraudCase.FraudCaseInterval)                    
                    .CreateListenerJob()
                    .Run();
            }            
        }
    }
}
