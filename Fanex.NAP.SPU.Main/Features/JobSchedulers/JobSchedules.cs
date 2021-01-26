namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    using Fanex.NAP.SPU.Main.ViewModels.Job;
    using System.Collections.Generic;

    public class JobSchedules
    {
        public JobSchedules(IEnumerable<JobScheduleViewModel> jobScheduleList)
        {
            JobScheduleList = jobScheduleList;
        }

        public IEnumerable<JobScheduleViewModel> JobScheduleList { get; }
    }
}
