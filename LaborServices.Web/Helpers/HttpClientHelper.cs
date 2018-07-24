using System;
using System.Net.Http;
using System.Web;

namespace LaborServices.Web.Helpers
{
    public class HttpClientHelper
    {
        public static HttpClient GetHttpClient(string url)
        {
            var myHttpClient = new HttpClient();
            var token = HttpContext.Current.Session["token"];

            //if (_token == null) throw new ArgumentNullException(nameof(_token));
            myHttpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Basic UGFzc05BU0FQSUBOYXNBUElVc2VyMTIzQFBhc3M6TmFzQVBJVXNlcjEyM0B1c2Vy#" + url));
            return myHttpClient;
        }

        public static HttpClient PostHttpClient(string url)
        {
            var myHttpClient = new HttpClient();
            var token = HttpContext.Current.Session["token"];

            myHttpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Basic UGFzc05BU0FQSUBOYXNBUElVc2VyMTIzQFBhc3M6TmFzQVBJVXNlcjEyM0B1c2Vy#" + url));
            return myHttpClient;
        }

    }
}