using Fanex.NAP.FPS.Renovation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Services.Interface
{
    public interface IUserAgent
    {
        List<UserAgentModel> GetUserAgent(int size);

        int UpdateUserAgent(string userAgentJson);
    }
}
