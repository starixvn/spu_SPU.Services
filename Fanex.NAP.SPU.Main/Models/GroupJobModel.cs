namespace Fanex.NAP.SPU.Main.Models
{
    using System.Collections.Generic;
    using System.Linq;    

    public class GroupJobModel
    {
        public GroupJobModel()
        {
            Jobs = Enumerable.Empty<JobModel>();
        }

        public IEnumerable<JobModel> Jobs { get; set; }
    }
}
