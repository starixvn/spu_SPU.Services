using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.SPU.ExecutingService
{
    public interface IJobFS
    {
        IJobFS CreateJob(string className, string jobName, string groupName, JobData jobData);

        IJobFS CreateTrigger(string triggerName, int interval);        

        IJobFS CreateSchedule();

        IJobFS CreateListenerJob();

        void Run();
    }
}