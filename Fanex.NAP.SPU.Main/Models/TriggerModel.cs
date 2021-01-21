namespace Fanex.NAP.SPU.Main.Models
{
    public class TriggerModel
    {
        public string TriggerType { get; set; }

        public string TriggerName { get; set; }

        public string Description { get; set; }

        public int RepeatInterval { get; set; }

        public string CronExpression { get; set; }

    }
}
