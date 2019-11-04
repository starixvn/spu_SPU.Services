using Fanex.NAP.FPS.Renovation.Model;
using Fanex.NAP.FPS.Renovation.Services.Interface;
using System.Collections.Generic;
using System.Linq;
using UAParser;
using Xunit;

namespace Fanex.NAP.SPU.Test
{
    public class UserAgentServiceTests
    {
        private readonly IUserAgent _userAgentService;
        public UserAgentServiceTests()
        {
            _userAgentService = new FPS.Renovation.Services.Implement.UserAgent();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ConvertUserAgent_ReturnNull(string values)
        {
            var result = _userAgentService.ConvertUserAgent(values);

            Assert.Null(result);
        }

        [Fact]
        public void ConvertUserAgent_ReturnCorrectData()
        {
            var userAgent = "Mozilla/5.0 (Linux; Android 8.1.0; SM-G610F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.67 Mobile Safari/537.36";

            var result = _userAgentService.ConvertUserAgent(userAgent);

            Assert.Equal("Android", result.OS.Family);
        }

        [Theory]
        [InlineData(null)]
        public void TransformUserAgent_ReturnEmptyList(List<UserAgentModel> values)
        {
            var result = _userAgentService.TransformUserAgent(values);

            Assert.False(result.Any());
        }
        [Fact]
        public void TransformUserAgent_ReturnCorrectListData()
        {
            var data = new List<UserAgentModel>();
            data.Add(new UserAgentModel
            {
                UserAgent = "Mozilla/5.0 (Linux; Android 8.1.0; SM-G610F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.67 Mobile Safari/537.36"
            });
            data.Add(new UserAgentModel
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_2_5 like Mac OS X) AppleWebKit/604.5.6 (KHTML, like Gecko) Version/11.0 Mobile/15D60 Safari/604.1"
            });

            var results = _userAgentService.TransformUserAgent(data);

            Assert.True(results.Count > 0);
            Assert.Equal("Android", results.Select(x => x.OSName).FirstOrDefault());
            Assert.Equal("Chrome Mobile", results.Select(x => x.BrowserName).FirstOrDefault());
        }

    }
}
