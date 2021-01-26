using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers
{
    public class JobRequest: IRequest<bool>
    {
        public string Name { get; set; }

        public JobRequest(string name)
        {
            this.Name = name;
        }
    }
}
