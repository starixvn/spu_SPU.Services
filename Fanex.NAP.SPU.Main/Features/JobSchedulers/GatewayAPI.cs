namespace Fanex.NAP.SPU.Main.Features.JobSchedulers
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Refit;
    using System;
    using System.Threading.Tasks;

    public interface IJobServiceApi
    {
        [Get("/Scheduler")]
        Task<SchedulerModel> GetScheduler();

        [Post("/job/{name}")]
        Task<bool> Execute(string name);
    }

    public class GatewayAPI : IJobServiceApi
    {
        public Task<bool> Execute(string name)
        {
            throw new NotImplementedException();
        }

        public Task<SchedulerModel> GetScheduler()
        {            
            throw new NotImplementedException();
        }
    }
}
