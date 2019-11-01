namespace Fanex.NAP.SPU.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.Service;
    using Fanex.NAP.SPU.Model;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FraudSensorJob : ScheduleJob
    {
        private const string groupName = "FraudSensor";
        private List<IScheduler> schedulers;

        public override void CreateScheduleJob<T>(JobDataSevices jobData)
        {
            var fraudCaseService = new FraudCaseService();
            var fraudCases = fraudCaseService.GetFraudCase<FS.Model.FraudCase>(null, 1, AppSetting.Environment);

            schedulers = new List<IScheduler>();

            foreach (var fraudCase in fraudCases)
            {
                if (fraudCase.IsActive)
                {
                    var job = new JobBase();
                    var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);
                    var jobName = string.Format("{0}Job", fraudCaseName);
                    var triggerName = string.Format("{0}Trigger", fraudCaseName);
                    var mapData = GetJobData(fraudCase.FraudCaseId, jobData);
                    
                    job.CreateJob<T>(jobName, groupName, mapData)
                        .CreateTriggerIntervalInSeconds(triggerName, groupName, fraudCase.FraudCaseInterval)
                        .CreateSchedule()
                        .CreateListenerJob()
                        .Run();

                    schedulers.Add(job.Schedule);
                }
            }            
        }

        public override void Shutdown()
        {
            if(schedulers.Any())
            {
                schedulers.ForEach(x => x.Shutdown(false));
            }
        }
    }
}
