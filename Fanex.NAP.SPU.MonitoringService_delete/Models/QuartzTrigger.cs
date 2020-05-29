namespace Fanex.NAP.SPU.MonitoringService.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class QuartzTrigger
    {
        public string JobGroup { get; set; }

        public string JobName { get; set; }

        public string TriggerName { get; set; }

        public Int64 NextFireTime { get; set; }

        public Int64 PrevFireTime { get; set; }

        public Int64 StartTime { get; set; }

        public int EndTime { get; set; }

        public int RepeatInterval { get; set; }

        public DateTime DisplayLastFireTime
        {
            get => new DateTime(PrevFireTime);
        }
    }
}
