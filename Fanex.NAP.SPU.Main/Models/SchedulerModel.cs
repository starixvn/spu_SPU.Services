namespace Fanex.NAP.SPU.Main.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class SchedulerModel
    {
        public SchedulerModel()
        {
            GroupJobs = Enumerable.Empty<GroupJobModel>();
        }

        public IEnumerable<GroupJobModel> GroupJobs { get; set; }
    }
}
