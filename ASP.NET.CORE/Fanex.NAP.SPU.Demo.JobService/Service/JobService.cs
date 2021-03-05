namespace Fanex.NAP.SPU.Demo.JobService.Service
{
    using System.Threading.Tasks;

    public interface IJobService
    {
        Task<bool> Execute(string jobName);
    }
    public class JobService : IJobService
    {        
        public Task<bool> Execute(string jobName)
        {
            return Task.FromResult(true);
        }
    }
}
