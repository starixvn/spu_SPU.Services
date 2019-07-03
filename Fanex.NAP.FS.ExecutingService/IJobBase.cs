using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.SPU.ExecutingService
{
    public interface IJobBase
    {
        IJobBase CreateJob(string className, string jobName, string groupName, JobData jobData);

        IJobBase CreateTrigger(string triggerName, int interval);        

        IJobBase CreateSchedule();

        IJobBase CreateListenerJob();

        void Run();
    }
}