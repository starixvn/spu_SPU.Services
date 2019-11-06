using Fanex.NAP.SPU.Model;

namespace Fanex.NAP.SPU.ExecutingService
{
    public interface IJobBase
    {
        IJobBase CreateJob<T>(JobData jobData);

        IJobBase DeleteJob(string jobName);

        IJobBase CreateTriggerIntervalInSeconds(int interval);

        IJobBase CreateTriggerWithCronSchedule(string expression);

        IJobBase GetSchedule();

        IJobBase CreateListenerJob();

        void Run();

        void Shutdown();                
    }
}