namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using Refit;
    using System.Threading.Tasks;

    public interface IJobService
    {
        Task<bool> Execute(string name, string apiUrl);
    }        

    public class JobService : IJobService
    {        
        public async Task<bool> Execute(string name, string apiUrl)
        {
            var jobServiceAPI = RestService.For<IJobServiceApi>(apiUrl);

            return await jobServiceAPI.Execute(name);
        }
    }
}
