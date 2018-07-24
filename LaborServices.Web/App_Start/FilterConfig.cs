using System.Web.Mvc;
using LaborServices.Web.filters;
using LaborServices.Web.Filters;

namespace LaborServices.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new AuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new PermissionsActionFilter(), 0);
            filters.Add(new HttpsFilter());
            filters.Add(new ExceptionFilter());
        }
    }
}
