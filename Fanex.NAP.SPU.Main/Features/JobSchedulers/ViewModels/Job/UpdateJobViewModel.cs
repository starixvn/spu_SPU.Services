namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job
{
    using System.Collections.Generic;    

    public class UpdateJobViewModel
    {
        public UpdateJobViewModel(IEnumerable<JobScheduleViewModel> newList
            , IEnumerable<JobScheduleViewModel> updatedList
            , IEnumerable<JobScheduleViewModel> deletedList)
        {
            NewList = newList;
            UpdatedList = updatedList;
            DeletedList = deletedList;
        }

        public IEnumerable<JobScheduleViewModel> NewList { get; set; }

        public IEnumerable<JobScheduleViewModel> UpdatedList { get; set; }

        public IEnumerable<JobScheduleViewModel> DeletedList { get; set; }
        
    }
}