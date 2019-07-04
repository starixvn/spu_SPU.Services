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
        public override void CreateScheduleJob(JobDataSevices jobData)
        {
            var fraudCaseService = new FraudCaseService();
            var fraudCases = fraudCaseService.GetFraudCase<FS.Model.FraudCase>(null, 1, AppSetting.Environment);

            foreach (var fraudCase in fraudCases)
            {
                if (fraudCase.IsActive)
                {
                    var job = new JobBase();
                    var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);
                    var jobName = string.Format("{0}Job", fraudCaseName);
                    var triggerName = string.Format("{0}Trigger", fraudCaseName);
                    var mapData = GetJobData(fraudCase.FraudCaseId, jobData);

                    job.CreateJob(jobName, groupName, mapData)
                        .CreateTrigger(triggerName, groupName, fraudCase.FraudCaseInterval)
                        .CreateSchedule()
                        .CreateListenerJob()
                        .Run();
                }
            }
        }
    }
}
