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

        [Post("/job/execute")]
        Task<bool> Execute(string name);
    }    
}
