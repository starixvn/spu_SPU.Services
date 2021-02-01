namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job
{
    using System.Collections.Generic;    

    public class UpdateJobViewModel
    {
        public UpdateJobViewModel(IEnumerable<JobScheduleViewModel> jobScheduleList)
        {
            JobScheduleList = jobScheduleList;
        }

        public IEnumerable<JobScheduleViewModel> JobScheduleList { get; set; }

    }
}
