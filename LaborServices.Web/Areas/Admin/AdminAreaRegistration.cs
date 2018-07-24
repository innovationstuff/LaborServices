using System.Web.Mvc;
using LaborServices.Utility;

namespace LaborServices.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "{lang}/Admin/{controller}/{action}/{id}",
                new { lang = AppConstants.DefaultLang, controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { lang = @"[a-z]{2}|[a-z]{2}-[a-zA-Z]{2}" },
                new[] { "LaborServices.Web.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                name: "Admin_defaultRoute",
                url: "Admin/{controller}/{action}/{id}",
                defaults: new { lang = AppConstants.DefaultLang, controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "LaborServices.Web.Areas.Admin.Controllers" }
            );
        }
    }
}