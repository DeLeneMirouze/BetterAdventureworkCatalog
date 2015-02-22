#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BetterIndexer
{
     class AzureSearchHelper:IDisposable
    {
        #region Constructor
        public AzureSearchHelper()
        {
            ApiVersionString = ConfigurationManager.AppSettings["Azure.Search.ApiVersionString"];
            client = new HttpClient();
            // Get the search service connection information from the App.config
            client.DefaultRequestHeaders.Add("api-key", ConfigurationManager.AppSettings["Azure.Search.ApiKey"]);
        } 
        #endregion

        #region SendSearchRequest
        public HttpResponseMessage SendSearchRequest(HttpMethod method, Uri uri, string json = null)
        {
            UriBuilder builder = new UriBuilder(uri);
            string separator = string.IsNullOrWhiteSpace(builder.Query) ? string.Empty : "&";
            builder.Query = builder.Query.TrimStart('?') + separator + ApiVersionString;

            var request = new HttpRequestMessage(method, builder.Uri);

            if (json != null)
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            Debug.WriteLine(builder.Uri);
            return client.SendAsync(request).Result;
        } 
        #endregion

        #region EnsureSuccessfulSearchResponse
        public void EnsureSuccessfulSearchResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string error = response.Content == null ? null : response.Content.ReadAsStringAsync().Result;
                throw new Exception("Search request failed: " + error);
            }
        } 
        #endregion

        private readonly string ApiVersionString;
        private readonly HttpClient client;

        #region Dispose
        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
            }
        } 
        #endregion
    }
}
