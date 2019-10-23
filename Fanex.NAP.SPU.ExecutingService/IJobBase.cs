using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.SPU.ExecutingService
{
    public interface IJobBase
    {
        IJobBase CreateJob(string jobName, string groupName, JobData jobData);

        IJobBase DeleteJob(string jobName, string groupName);

        IJobBase CreateTrigger(string triggerName, string groupName, int interval);

        IJobBase CreateTriggerWithCronSchedule(string triggerName, string groupName, string expression);

        IJobBase CreateSchedule();

        IJobBase CreateListenerJob();

        void Run();

        void Shutdown();
    }
}