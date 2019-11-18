using Fanex.Logging;
using Fanex.NAP.DCS.DataAccess;
using Fanex.NAP.DCS.Model;
using Fanex.NAP.DCS.Services.Interface;
using System.Collections.Generic;
using System.Linq;
using UAParser;

namespace Fanex.NAP.DCS.Services.Implement
{
    public class UserAgentService : IUserAgent
    {
        private readonly DataAccess.DataCenterDataAccess dataCenter;

        public UserAgentService()
        {
            dataCenter = new DataCenterDataAccess();
        }

        public ClientInfo ConvertUserAgent(string userAgent)
        {
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                var uaParser = Parser.GetDefault();

                return uaParser.Parse(userAgent);
            }

            return null;
        }

        public List<UserAgentModel> GetUserAgent(int size)
        {
            return dataCenter.GetUserAgent(size);           
        }

        public List<UserAgentModel> TransformUserAgent(List<UserAgentModel> userAgents)
        {
            if (userAgents != null && userAgents.Any())
            {
                return userAgents.Select(x =>
                {
                    var clientInfo = ConvertUserAgent(x.UserAgent);

                    if(clientInfo != null)
                    {
                        var browserName = clientInfo.UA.Family;
                        var osName = clientInfo.OS.Family;

                        x.OSName = osName;
                        x.BrowserName = browserName;
                    } 
                   
                    return x;

                }).ToList();
            }

            return new List<UserAgentModel>();
        }

        public int UpdateUserAgent(string userAgentJson)
        {
            return dataCenter.UpdateUserAgent(userAgentJson).ErrorCode;
        }
    }
}
