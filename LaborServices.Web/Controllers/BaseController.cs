using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LaborServices.Utility;
using LaborServices.Web.Helpers;
using System.Net;
using LaborServices.Web.Models;
using LaborServices.Entity;
using LaborServices.Managers;

namespace LaborServices.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        public Language Lang { get; set; }
        public string LangCode { get; set; }
        public SettingManager _settingManager;
        private int _pageSize = 10;

        public BaseController()
        {
            _settingManager = new SettingManager(new LaborServicesDbContext());
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string routeDataKey = "lang";
            const string defaultLanguageCode = AppConstants.DefaultLang;
            var validLanguageCodes = new[] { "en", "ar" };

            // Determine the language.
            if (filterContext.RouteData.Values[routeDataKey] == null ||
                !validLanguageCodes.Contains(filterContext.RouteData.Values[routeDataKey]))
            {
                LangCode = defaultLanguageCode;
                // Add or overwrite the langauge code value.
                if (filterContext.RouteData.Values.ContainsKey(routeDataKey))
                {
                    filterContext.RouteData.Values[routeDataKey] = defaultLanguageCode;
                }
                else
                {
                    filterContext.RouteData.Values.Add(routeDataKey, defaultLanguageCode);
                }

                Lang = defaultLanguageCode.ToLower() == "en" ? Language.English : Language.Arabic;
                Session["Language"] = Lang;
            }
            else
            {
                var langCode = filterContext.RouteData.Values[routeDataKey];
                Lang = langCode != null && langCode.ToString().ToLower() == "en" ? Language.English : Language.Arabic;
                Session["Language"] = Lang;
                LangCode = langCode.ToString().ToLower();

            }

            ViewBag.Lang = Lang;
            ViewBag.LangCode = LangCode;

            base.OnActionExecuting(filterContext);
        }

        #region Calling api methods

        protected async Task<T> GetResourceAsync<T>(string url, params string[] paramters)
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

        protected async Task<APIResponseModel<T>> GetResourceMessageAsync<T>(string url, params string[] paramters)
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

        protected async Task<APIResponseModel<T>> PostResourceAsync<T>(string url, object content)
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

        protected async Task<APIResponseModel<T>> UpdateResourceAsync<T>(string url, object content)
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

        protected async Task<HttpStatusCode> DeleteResourceAsync(string url)
        {
            using (var client = new HttpClient())
            {
                string apiServiceUrl = SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar") + "/";

                client.BaseAddress = new Uri(apiServiceUrl);
                var response = await client.DeleteAsync(url);
                return response.StatusCode;
            }
        }
        #endregion

        #region  sms 

        protected async Task<bool> SendSMSAsync(Microsoft.AspNet.Identity.IdentityMessage message)
        {
            string url = string.Format("api/SMS/Send?Message={0}&MobileNumber={1}", message.Body, message.Destination);
            var result = await GetResourceMessageAsync<string>(url);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var isSent = Convert.ToInt32(result.Result) == 1;
                return isSent;
            }
            return false;
        }

        #endregion

        #region shared lookups



        [AllowAnonymous]
        public async Task<JsonResult> GetLookup(string url)
        {
            var data = await GetResourceAsync<dynamic>(url);
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                var context = new LaborServicesDbContext();
                var username = User.Identity.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var user = context.Users.SingleOrDefault(u => u.UserName == username);
                    //using (var client = new HttpClient())
                    //{
                    //    string apiServiceUrl = SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar") + "/api/contact/" + user.CrmUserId;

                    //    //Passing service base url  
                    //    client.BaseAddress = new Uri(apiServiceUrl);

                    //    client.DefaultRequestHeaders.Clear();
                    //    //Define request data format  
                    //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



                    //    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    //    var response = client.GetAsync(apiServiceUrl);

                    //    //Checking the response is successful or not which is sent using HttpClient  
                    //    if (response.Result.IsSuccessStatusCode)
                    //    {
                    //        //Storing the response details recieved from web api   
                    //        var data = response.Result.Content.ReadAsAsync<ContactViewModel>();


                    //        ContactViewModel userloggedIn = data == null ? new ContactViewModel() : (ContactViewModel)data.Result;
                    //        string fullName = userloggedIn.FirstName;
                    //        ViewData.Add("FullName", fullName);
                    //    }
                    //}
                    if (user != null && user.Name != null)
                    {
                        var names = user.Name.Split(' ').ToList();
                        string fullName = "";
                        if (names.Count >= 2)
                        {
                            fullName = names[0] + " " + names[1];
                        }
                        else
                            fullName = user.Name;
                        ViewData.Add("FullName", fullName);
                    }
                    else
                    {
                        ViewData.Add("FullName", "");
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }

    }
}