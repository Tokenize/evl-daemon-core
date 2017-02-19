using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvlDaemon.Http
{
    /// <summary>
    /// A simple HTTP client that can be used as a base for
    /// concrete HTTP clients.
    /// </summary>
    public abstract class BaseHttpClient
    {
        private HttpClient httpClient;

        public string BaseUrl { get; set; }
        protected string Username { get; set; }
        protected string Password { get; set; }

        public BaseHttpClient(string username, string password)
        {
            httpClient = new HttpClient();

            var authHeaderBytes = Encoding.ASCII.GetBytes($"{username}:{password}");
            var authHeaderString = Convert.ToBase64String(authHeaderBytes);

            var authHeader = new AuthenticationHeaderValue("Basic", authHeaderString);
            httpClient.DefaultRequestHeaders.Authorization = authHeader;
        }

        public BaseHttpClient()
        {
            httpClient = new HttpClient();
        }

        /// <summary>
        /// Post the given data as key value pairs to the given path.
        /// </summary>
        /// <param name="path">Path relative to the base URL</param>
        /// <param name="data">Data to post</param>
        public async Task<HttpResponseMessage> PostAsync(string path, IEnumerable<KeyValuePair<string, string>> data)
        {
            HttpContent content = new FormUrlEncodedContent(data);
            return await httpClient.PostAsync($"{BaseUrl}{path}", content);
        }
    }
}
