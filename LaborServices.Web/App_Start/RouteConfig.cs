using System.Web.Mvc;
using System.Web.Routing;
using LaborServices.Utility;

namespace LaborServices.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();


            //routes.MapRoute(
            //name: "IndividualPayment",
            //url: "{lang}/Payment/Individual/{action}/{id}",
            //defaults: new { lang = AppConstants.DefaultLang, controller = "Payment", action = "IndividualPaymentMethod", id = UrlParameter.Optional },
            //constraints: new { lang = @"[a-z]{2}|[a-z]{2}-[a-zA-Z]{2}" },
            //namespaces: new[] { "LaborServices.Web.Controllers" });


            routes.MapRoute(
                name: "LocalizedDefault",
                url: "{lang}/{controller}/{action}/{id}",
                defaults: new { lang = AppConstants.DefaultLang, controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { lang = @"[a-z]{2}|[a-z]{2}-[a-zA-Z]{2}" },
                namespaces: new[] { "LaborServices.Web.Controllers" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { lang = AppConstants.DefaultLang, controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "LaborServices.Web.Controllers" }
            );


        }
    }
}
