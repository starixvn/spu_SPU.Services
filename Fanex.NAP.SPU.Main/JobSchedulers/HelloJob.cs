using Quartz;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.JobSchedulers
{
    public class HelloJob : IJob
    {        
        public Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
