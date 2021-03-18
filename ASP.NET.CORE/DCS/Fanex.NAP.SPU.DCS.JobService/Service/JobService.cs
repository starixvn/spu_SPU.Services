using Sentry;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Service
{
    public interface IJobService
    {
        Task<bool> Execute(string jobName);
    }
    public class JobService : IJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IDataService dataService;

        public JobService(ISentryClient sentryClient, IDataService dataService)
        {
            this.sentryClient = sentryClient;
            this.dataService = dataService;
        }


        public Task<bool> Execute(string jobName)
        {
            //dataService.GetData(0);

            return Task.FromResult(true);
        }
    }
}
