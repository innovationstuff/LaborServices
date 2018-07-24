using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LaborServices.Managers.Identity;
using LaborServices.Model.Identity;
using LaborServices.Utility;
using Microsoft.AspNet.Identity;

namespace LaborServices.Web.Helpers
{
    /// <summary>
    /// Custom authorization attribute for setting per-method accessibility 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class SetPermissionsAttribute : AuthorizeAttribute
    {

        public string NameAr;
        public string NameEn;
        public string Controller;
        public string Action;
        public string Area;
        public bool IsBaseParent;
        public SetPermissionsAttribute(string nameAr, string nameEn, string controller, string action, string area, bool isBaseParent)
        {
            NameAr = nameAr;
            NameEn = nameEn;
            Controller = controller;
            Action = action;
            Area = area;
            IsBaseParent = isBaseParent;
        }

        //Called when access is denied
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            ////User isn't logged in
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login",  Area = "" }));
            }
            //User is logged in but has no access
            else
            {
                //filterContext.Result = new ViewResult() { ViewName = "AccessDenied" };

                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "Index",  Area = "" })
                );
            }
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            bool isUserAuthorized = base.AuthorizeCore(httpContext);
            httpContext.Items["NavbarItems"] = new List<ApplicationPage>();
            // httpContext.Session["NavbarItems"] = new List<ApplicationPage>();

            if (isUserAuthorized == false)
            {
                return false;
            }

            var pageManager = new ApplicationPageManager(httpContext.GetOwinContext());

            // admin have full access rights
            if (httpContext.User.IsInRole(AppConstants.AdminRoleName))
            {
                httpContext.Items["NavbarItems"] = pageManager.AdminPages.ToList();
                return true;
            }


            var rd = httpContext.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action").ToLower();
            string currentController = rd.GetRequiredString("controller").ToLower();
            var requestArea = httpContext.Request.RequestContext.RouteData.DataTokens["area"]?.ToString() ?? "";

            var currentArea = string.IsNullOrEmpty(requestArea) ? null : requestArea.ToLower();

            var currentUserId = httpContext.User.Identity.GetUserId();

            var navbarItems = pageManager.GetUserPages(currentUserId).ToList();

            httpContext.Items["NavbarItems"] = navbarItems;

            if (navbarItems == null || navbarItems.Any() == false) return false;


            var isAuthorized = navbarItems.Any(
                n => n.Controller.ToLower() == currentController &&
                     n.Action.ToLower() == currentAction &&
                     (n.Area == currentArea));
            return isAuthorized;
        }
    }
}