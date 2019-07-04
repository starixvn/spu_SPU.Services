using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.SPU.ExecutingService
{
    public interface IJobBase
    {
        IJobBase CreateJob(string jobName, string groupName, JobData jobData);

        IJobBase CreateTrigger(string triggerName, string groupName, int interval);        

        IJobBase CreateSchedule();

        IJobBase CreateListenerJob();

        void Run();
    }
}