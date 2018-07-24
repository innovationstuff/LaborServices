using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.filters
{
    public class HttpsFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsSecureConnection)
            {

                if (!filterContext.HttpContext.Request.Url.ToString().Contains("localhost"))
                {
                    var url = filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:");
                    filterContext.Result = new RedirectResult(url);
                }
            }
        }
    }
}