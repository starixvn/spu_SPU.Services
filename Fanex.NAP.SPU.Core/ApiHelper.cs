namespace Fanex.NAP.SPU.Core
{
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public static class ApiHelper
    {
        [Obsolete("Too many generic and parameter")]
        public static async Task<TResult> Post<TParam, TResult>(string url, TParam param, int environment, int mailingListId)
        {

            using (var web = new HttpClient())
            {
                var response = await web.PostAsJsonAsync(url, param).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    return default(TResult);
                }

                return await response.Content.ReadAsAsync<TResult>().ConfigureAwait(false);
            }

        }

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

        public static async Task Post(string url, object param)
        {
            using (var web = new HttpClient())
            {
                await web.PostAsJsonAsync(url, param).ConfigureAwait(false);
            }
        }

        public static async Task<TResult> Get<TResult>(string url)
        {
            using (var web = new HttpClient())
            {
                var response = await web.GetAsync(url).ConfigureAwait(false);

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
