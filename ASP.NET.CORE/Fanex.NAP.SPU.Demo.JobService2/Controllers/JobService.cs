using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Demo.JobService2.Controllers
{
    public interface IJobService
    {
        Task<bool> Update(string name);
    }
    public class JobService : IJobService
    {
        public Task<bool> Update(string name)
        {
            return Task.FromResult(true);
        }
    }
}
