namespace Fanex.NAP.SPU.Main.Models
{
    using System.Collections.Generic;

    public class JobModel
    {
        public string JobName { get; set; }

        public string Group { get; set; }

        public string Class { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<TriggerModel> Triggers { get; set; }

    }
}
