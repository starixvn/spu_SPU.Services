namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Models
{
    public class TriggerModel
    {
        public string TriggerType { get; set; }

        public string TriggerName { get; set; }        

        public bool RepeatForever { get; set; }

        public string CronExpression { get; set; }

    }
}
