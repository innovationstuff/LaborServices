using LaborServices.Utility;
using LaborServices.Web.Helpers;
using LaborServices.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LaborServices.Web.Managers
{
    public class ApiCaller
    {
        public Language Lang { get; set; }
        const string defaultLanguageCode = AppConstants.DefaultLang;
        public ApiCaller()
        {
            Lang = defaultLanguageCode.ToLower() == "en" ? Language.English : Language.Arabic;
        }
        public ApiCaller(Language lang)
        {
            this.Lang = lang;
        }

        public async Task<T> GetResourceAsync<T>(string url, params string[] paramters)
        {
            using (var client = new HttpClient())
            {
                string apiServiceUrl = SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar") + "/";

                //Passing service base url  
                client.BaseAddress = new Uri(apiServiceUrl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var urlParams = new StringBuilder();

                foreach (string param in paramters)
                {
                    urlParams.Append(param + "/");
                }

                var paramList = urlParams.ToString();
                var fullUrl = string.IsNullOrEmpty(paramList) ? url : url + paramList;

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                var response = await client.GetAsync(fullUrl);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                return default(T);
            }
        }

        public async Task<APIResponseModel<T>> GetResourceMessageAsync<T>(string url, params string[] paramters)
        {
            using (var client = new HttpClient())
            {
                string apiServiceUrl = SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar") + "/";

                //Passing service base url  
                client.BaseAddress = new Uri(apiServiceUrl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var urlParams = new StringBuilder();

                foreach (string param in paramters)
                {
                    urlParams.Append(param + "/");
                }

                var paramList = urlParams.ToString();
                var fullUrl = string.IsNullOrEmpty(paramList) ? url : url + paramList;

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                var response = await client.GetAsync(fullUrl);
                var result = new APIResponseModel<T>();
                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var responseContent = await response.Content.ReadAsAsync<T>();
                    result.Result = responseContent;
                    result.StatusCode = response.StatusCode;
                    return result;
                }
                result.Result = default(T);
                result.StatusCode = response.StatusCode;
                result.StatusMessage = response.CreateApiException();
                return result;
            }
        }

        public async Task<APIResponseModel<T>> PostResourceAsync<T>(string url, object content)
        {
            using (var client = new HttpClient())
            {
                string apiServiceUrl = SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar") + "/";

                client.BaseAddress = new Uri(apiServiceUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Model will be serialized automatically.
                var response = await client.PostAsJsonAsync(url, content);
                var result = new APIResponseModel<T>();

                if (response.IsSuccessStatusCode)
                {
                    //ReadAsAsync permits to deserialize the response content
                    var responseContent = await response.Content.ReadAsAsync<T>();
                    result.Result = responseContent;
                    result.StatusCode = response.StatusCode;
                    return result;
                }
                result.Result = default(T);
                result.StatusCode = response.StatusCode;
                result.StatusMessage = response.CreateApiException();
                return result;
            }
        }

        public async Task<APIResponseModel<T>> UpdateResourceAsync<T>(string url, object content)
        {
            using (var client = new HttpClient())
            {
                string apiServiceUrl = SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar") + "/";

                client.BaseAddress = new Uri(apiServiceUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Model will be serialized automatically.
                var response = await client.PutAsJsonAsync(url, content);
                var result = new APIResponseModel<T>();

                if (response.IsSuccessStatusCode)
                {
                    //ReadAsAsync permits to deserialize the response content
                    var responseContent = await response.Content.ReadAsAsync<T>();
                    result.Result = responseContent;
                    result.StatusCode = response.StatusCode;
                    return result;
                }
                result.Result = default(T);
                result.StatusCode = response.StatusCode;
                result.StatusMessage = response.CreateApiException();
                return result;
            }
        }

        public async Task<HttpStatusCode> DeleteResourceAsync(string url)
        {
            using (var client = new HttpClient())
            {
                string apiServiceUrl = SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar") + "/";

                client.BaseAddress = new Uri(apiServiceUrl);
                var response = await client.DeleteAsync(url);
                return response.StatusCode;
            }
        }


        public async Task<HttpResponseMessage> PostFileAsync(string url, HttpPostedFileBase File)
        {
            using (var client = new HttpClient())
            {
                string apiServiceUrl = String.Format("{0}/{1}", SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar"), url);

                using (var content =
                    new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    content.Add(new StreamContent(File.InputStream), "File", File.FileName);

                    return await client.PostAsync(apiServiceUrl, content);

                }
            }
        }


    }
}