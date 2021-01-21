using System.Collections.Generic;

namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    public class JobSchedules
    {
        public JobSchedules(IEnumerable<JobSchedule> jobScheduleList)
        {
            JobScheduleList = jobScheduleList;
        }

        public IEnumerable<JobSchedule> JobScheduleList { get; }
    }
}
