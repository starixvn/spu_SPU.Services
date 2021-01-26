namespace Fanex.NAP.SPU.Gateway.Models
{
    using System.Collections.Generic;
    using System.Linq; 

    public class GroupJobModel
    {       
        public IEnumerable<JobModel> Jobs { get; set; }
    }
}
