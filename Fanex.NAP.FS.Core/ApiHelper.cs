#pragma warning disable S3994 // URI Parameters should not be strings
#pragma warning disable S112 // General exceptions should never be thrown
#pragma warning disable S4005 // "System.Uri" arguments should be used instead of strings

namespace Fanex.NAP.FS.Core
{
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public static class ApiHelper
    {
        public static async Task<TResult> Post<TResult>(string url, object param)
        {
            using (var web = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(param);

                var buffer = Encoding.UTF8.GetBytes(content);

                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await web.PostAsync(url, byteContent).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }
    }
}

#pragma warning restore S4005 // "System.Uri" arguments should be used instead of strings
#pragma warning restore S112 // General exceptions should never be thrown
#pragma warning restore S3994 // URI Parameters should not be strings