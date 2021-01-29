namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    public class TriggerViewModel
    {
        public string DateFormat { get; }
        public string TimeFormat { get; }
        public string DateTimeFormat { get => DateFormat + " " + TimeFormat; }

        [Required]
        public int? MisfireInstruction { get; set; }
                
        public CronTriggerViewModel Cron { get; set; } = new CronTriggerViewModel();

        public void Validate(ICollection<ValidationError> errors) { }
        
    }
}
