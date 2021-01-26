namespace Fanex.NAP.SPU.Gateway.Models
{
    using System.Collections.Generic;

    public class GroupJobModel
    {
        public string GroupName { get; set; }

        public IEnumerable<JobModel> Jobs { get; set; }
    }
}