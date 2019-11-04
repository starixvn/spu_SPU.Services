using Fanex.NAP.FPS.Renovation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NAP.SPU.Test
{
    public class CoreTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void EncryptString_ReturnNull(string values)
        {
            var result = Cipher.Encrypt(values);

            Assert.Null(result);
        }

        [Fact]
        public void EncryptString_ReturnCorrect()
        {
            var result = Cipher.Encrypt("testing");
            var expected = "DA57Nuc1ZiV2JU7Wir/fIw==";

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void DecryptString_ReturnNull(string values)
        {
            var result = Cipher.Decrypt(values);

            Assert.Null(result);
        }

        [Fact]
        public void DecryptString_ReturnCorrect()
        {
            var result = Cipher.Decrypt("DA57Nuc1ZiV2JU7Wir/fIw==");
            var expected = "testing";

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetURLFromDeviceCode_ReturnNull(string values)
        {
            var result = Utilities.GetURLFromDeviceCode(values);

            Assert.Equal(string.Empty,result);
        }

        [Fact]
        public void GetURLFromDeviceCode_ReturnCorrect()
        {
            var data = "Jonsnow776|login|login -> successfully|application/json,text/javascript,*/*; q=0.01###Unknown###240###Mozilla/5.0 (Linux; Android 7.1.2; vivo 1611) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Mobile Safari/537.36###640:480:8:false###103.214.60.238###eyJuYSI6Ik4vQSIsImRldmljZUNvZGUiOiJlMDNiYTY1ZTJkMDAzZTBkN2E3ZGJiNDFhYjdjMmI3NjFlNDdjOGUyO2Q4YTMwMGUyM2QzOTk1OTVhODRlOWM2N2NhNDc3M2M3IiwiYXBwVmVyc2lvbiI6IjUuMCAoTGludXg7IEFuZHJvaWQgNy4xLjI7IHZpdm8gMTYxMSkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzc0LjAuMzcyOS4xNTcgTW9iaWxlIFNhZmFyaS81MzcuMzYiLCJ0aW1lWm9uZSI6Ii0zMzAiLCJ1c2VyQWdlbnQiOiJNb3ppbGxhLzUuMCAoTGludXg7IEFuZHJvaWQgNy4xLjI7IHZpdm8gMTYxMSkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzc0LjAuMzcyOS4xNTcgTW9iaWxlIFNhZmFyaS81MzcuMzYiLCJzY3JlZW4iOnsid2lkdGgiOjM2MCwiaGVpZ2h0Ijo2NDAsImNvbG9yRGVwdGgiOjI0fSwiZGV2aWNlSWQiOiIzMzdkYWE0ODU2ODM0NGQ2YWIyNGZjYzk3MmE1MDEzYSIsImhyZWYiOiJodHRwczovL2lwaG9uZS4xMmJldC5jb20vI1Nwb3J0cy9zPTFfMCZkPXQiLCJjYXB0dXJlZERhdGUiOiI2MzY5NTk5MjM4ODc5NzI0NjMiLCJnZW9sb2NhdGlvbiI6IiIsInZlcnNpb24iOiIxLjEuOSJ9###http://iphone.12bet.com:801/FpsHandler###|103.214.60.238|12BET|6/13/2019 3:13:09 AM||6/13/2019 3:13:09 AM";
            
            var result = Utilities.GetURLFromDeviceCode(data);

            var expected = "http://iphone.12bet.com:801/FpsHandler";

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetUserAgentFromDeviceCode_ReturnNull(string values)
        {
            var result = Utilities.GetUserAgentFromDeviceCode(values);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetUserAgentFromDeviceCode_ReturnCorrect()
        {
            var data = "Jonsnow776|login|login -> successfully|application/json,text/javascript,*/*; q=0.01###Unknown###240###Mozilla/5.0 (Linux; Android 7.1.2; vivo 1611) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Mobile Safari/537.36###640:480:8:false###103.214.60.238###eyJuYSI6Ik4vQSIsImRldmljZUNvZGUiOiJlMDNiYTY1ZTJkMDAzZTBkN2E3ZGJiNDFhYjdjMmI3NjFlNDdjOGUyO2Q4YTMwMGUyM2QzOTk1OTVhODRlOWM2N2NhNDc3M2M3IiwiYXBwVmVyc2lvbiI6IjUuMCAoTGludXg7IEFuZHJvaWQgNy4xLjI7IHZpdm8gMTYxMSkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzc0LjAuMzcyOS4xNTcgTW9iaWxlIFNhZmFyaS81MzcuMzYiLCJ0aW1lWm9uZSI6Ii0zMzAiLCJ1c2VyQWdlbnQiOiJNb3ppbGxhLzUuMCAoTGludXg7IEFuZHJvaWQgNy4xLjI7IHZpdm8gMTYxMSkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzc0LjAuMzcyOS4xNTcgTW9iaWxlIFNhZmFyaS81MzcuMzYiLCJzY3JlZW4iOnsid2lkdGgiOjM2MCwiaGVpZ2h0Ijo2NDAsImNvbG9yRGVwdGgiOjI0fSwiZGV2aWNlSWQiOiIzMzdkYWE0ODU2ODM0NGQ2YWIyNGZjYzk3MmE1MDEzYSIsImhyZWYiOiJodHRwczovL2lwaG9uZS4xMmJldC5jb20vI1Nwb3J0cy9zPTFfMCZkPXQiLCJjYXB0dXJlZERhdGUiOiI2MzY5NTk5MjM4ODc5NzI0NjMiLCJnZW9sb2NhdGlvbiI6IiIsInZlcnNpb24iOiIxLjEuOSJ9###http://iphone.12bet.com:801/FpsHandler###|103.214.60.238|12BET|6/13/2019 3:13:09 AM||6/13/2019 3:13:09 AM";

            var result = Utilities.GetUserAgentFromDeviceCode(data);

            var expected = "Mozilla/5.0 (Linux; Android 7.1.2; vivo 1611) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Mobile Safari/537.36";

            Assert.Equal(expected, result);
        }

    }
}
