namespace Fanex.NAP.SPU.Main.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class SchedulerModel
    {
        public SchedulerModel()
        {
            Jobs = Enumerable.Empty<JobModel>();
        }

        public IEnumerable<JobModel> Jobs { get; set; }
    }
}
