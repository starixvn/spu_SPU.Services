using Refit;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers
{
    public interface IJobService
    {
        Task<bool> Execute(string name);
    }

    public interface IJobServiceApi
    {
        [Post("/job/execute")]
        Task<bool> Execute(string name);
    }

    public class JobService : IJobService
    {
        private readonly IJobServiceApi jobServiceAPI;

        public JobService(IJobServiceApi jobServiceAPI)
        {
            this.jobServiceAPI = jobServiceAPI;
        }

        public async Task<bool> Execute(string name)
        {
            return await jobServiceAPI.Execute(name);
        }
    }
}
