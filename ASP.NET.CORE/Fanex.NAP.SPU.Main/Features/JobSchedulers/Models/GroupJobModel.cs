namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Models
{
    using System.Collections.Generic;
    using System.Linq;    

    public class GroupJobModel
    {
        public GroupJobModel()
        {
            Jobs = Enumerable.Empty<JobModel>();
        }

        public string GroupName { get; set; }

        public IEnumerable<JobModel> Jobs { get; set; }
    }
}
