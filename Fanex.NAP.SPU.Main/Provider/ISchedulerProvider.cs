namespace Fanex.NAP.SPU.Main.Provider
{    
    using Quartz;

    public interface ISchedulerProvider
    {
        IScheduler CreateScheduler();
    }
}
