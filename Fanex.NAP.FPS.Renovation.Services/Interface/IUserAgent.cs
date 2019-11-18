using Fanex.NAP.DCS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAParser;

namespace Fanex.NAP.DCS.Services.Interface
{
    public interface IUserAgent
    {
        List<UserAgentModel> GetUserAgent(int size);

        int UpdateUserAgent(string userAgentJson);

        ClientInfo ConvertUserAgent(string userAgent);

        List<UserAgentModel> TransformUserAgent(List<UserAgentModel> userAgents);
    }
}
