#region using
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

#endregion

namespace DemoConcept
{
    public sealed class AzureSearch
    {
        #region Constructor
        public AzureSearch()
        {
            ServiceUri = new Uri("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net");
            VersionString = ConfigurationManager.AppSettings["ApiVersionString"];
            ApiKey = ConfigurationManager.AppSettings["SearchServiceApiKey"];
        }
        #endregion

        #region SendRequest
        public WebResponse SendRequest(string method, string uriDef, string json = null)
        {
            Uri uri = GetRequestUri(uriDef);
            // for debuging purpose
            Debug.WriteLine(uri);

            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Method = method;
            webRequest.Headers.Add("api-key", ApiKey);

            if (json != null)
            {
                webRequest.ContentType = "application/json";
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                webRequest.ContentLength = bytes.Length;

                // push json into the request's object stream
                using (Stream stream = webRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            // run the request
            WebResponse response = webRequest.GetResponse();
            return response;
        }
        #endregion

        #region GetRequestUri (private)
        private Uri GetRequestUri(string uriDef)
        {
            Uri uri = new Uri(ServiceUri, uriDef);
            UriBuilder builder = new UriBuilder(uri);

            string separator = string.IsNullOrWhiteSpace(builder.Query) ? string.Empty : "&";
            builder.Query = string.Concat(builder.Query.TrimStart('?'), separator, VersionString);

            return builder.Uri;
        } 
        #endregion

        private Uri ServiceUri { get; set; }
        private string VersionString { get; set; }
        private string ApiKey { get; set; }
    }
}
