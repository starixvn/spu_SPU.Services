namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using System.Collections.Generic;   

   
    public class JobSchedules
    {
        public JobSchedules(List<JobScheduleViewModel> jobScheduleList)
        {
            JobScheduleList = jobScheduleList;
        }

        public List<JobScheduleViewModel> JobScheduleList { get; set; }

        public void UpdateJobScheduleList(IEnumerable<JobScheduleViewModel> jobScheduleList)
        {
            this.JobScheduleList.Clear();

            this.JobScheduleList.AddRange(jobScheduleList);
        }
    }
}
