namespace Fanex.NAP.SPU.Main.Features.JobSchedulers
{
    using Refit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IJobService
    {
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
