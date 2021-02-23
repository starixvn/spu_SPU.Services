namespace Fanex.NAP.SPU.Gateway.Models
{
    using System.Collections.Generic;

    public class JobModel
    {
        public string JobName { get; set; }

        public string JobType { get; set; }                

        public string APIUrl { get; set; }
       
        public string HealthCheckUrl { get; set; }

        public string Active { get; set; }

        public bool IsActive { 
            get 
            {
                var isActive = true;

                if (!string.IsNullOrEmpty(this.Active))
                {
                    bool.TryParse(this.Active, out isActive);
                }

                return isActive;
            }         
        }

        public IEnumerable<TriggerModel> Triggers { get; set; }
    }
}
