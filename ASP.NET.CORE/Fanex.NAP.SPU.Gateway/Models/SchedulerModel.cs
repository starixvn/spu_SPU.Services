namespace Fanex.NAP.SPU.Gateway.Models
{
    using System.Collections.Generic;

    public class SchedulerModel
    {
        public IEnumerable<JobModel> Jobs { get; set; }
    }
}
