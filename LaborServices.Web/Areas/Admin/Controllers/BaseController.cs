using System.Linq;
using System.Web.Mvc;
using LaborServices.Utility;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    public abstract class BaseController : Controller
    {
        public Language Language { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string routeDataKey = "lang";
            const string defaultLanguageCode = AppConstants.DefaultLang;
            var validLanguageCodes = new[] { "en", "ar" };

            // Determine the language.
            if (filterContext.RouteData.Values[routeDataKey] == null ||
                !validLanguageCodes.Contains(filterContext.RouteData.Values[routeDataKey]))
            {
                // Add or overwrite the langauge code value.
                if (filterContext.RouteData.Values.ContainsKey(routeDataKey))
                {
                    Language = defaultLanguageCode == "en" ? Language.English : Language.Arabic;

                    filterContext.RouteData.Values[routeDataKey] = defaultLanguageCode;
                    Session["Language"] = Language;
                }
                else
                {
                    filterContext.RouteData.Values.Add(routeDataKey, defaultLanguageCode);
                }
            }

            base.OnActionExecuting(filterContext);
        }

    }
}