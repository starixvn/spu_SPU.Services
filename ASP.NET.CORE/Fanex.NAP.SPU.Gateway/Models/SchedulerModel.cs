namespace Fanex.NAP.SPU.Gateway.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class SchedulerModel
    {
        public IEnumerable<GroupJobModel> GroupJobs { get; set; }
    }
}
