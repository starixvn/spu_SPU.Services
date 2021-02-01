namespace Fanex.NAP.SPU.Main
{
    public enum TriggerType
    {
        Unknown = 0,
        Cron,
        Simple,
        Daily,
        Calendar
    }

    public enum Unit
    {
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Year
    }

    public enum UpdateStatus
    {
        NoUpdate,
        Update
    }
    
}
