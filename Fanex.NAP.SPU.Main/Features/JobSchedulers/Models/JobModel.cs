namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Models
{
    using System.Collections.Generic;

    public class JobModel
    {
        public string JobName { get; set; }

        public string JobType { get; set; }

        public string APIUrl { get; set; }

        public IEnumerable<TriggerModel> Triggers { get; set; }

    }
}
