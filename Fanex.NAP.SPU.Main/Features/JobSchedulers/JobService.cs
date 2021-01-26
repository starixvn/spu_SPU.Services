using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers
{
    public interface IJobService
    {
        Task<bool> Execute(string name);
    }

    public interface IJobServiceApi
    {
        [Post("/job/{name}")]
        Task<bool> Execute(string name);
    }

    public class JobService : IJobService
    {
        private readonly IJobServiceApi jobServiceAPI;

        public JobService(IJobServiceApi jobServiceAPI)
        {
            this.jobServiceAPI = jobServiceAPI;
        }

        public  async Task<bool> Execute(string name)
        {
            return await this.jobServiceAPI.Execute(name);
        }
    }
}
