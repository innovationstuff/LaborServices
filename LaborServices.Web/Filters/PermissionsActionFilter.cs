using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LaborServices.Model.Identity;
using LaborServices.Utility;
using LaborServices.Web.Helpers;
using Microsoft.AspNet.Identity;
using LaborServices.Managers.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LaborServices.Web.filters
{
    public class PermissionsActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var currentUser = filterContext.HttpContext.User;

            var navbarItems = new List<ApplicationPage>();

            if (currentUser.Identity.IsAuthenticated)
            {
                var isAdmin = currentUser.IsInRole(AppConstants.AdminRoleName);
               // navbarItems = filterContext.HttpContext.GetNavbarItems();

                var pageManager = new ApplicationPageManager(filterContext.HttpContext.Request.GetOwinContext());

                navbarItems = isAdmin ? 
                    pageManager.AdminPages.ToList() :
                    pageManager.GetUserPages(currentUser.Identity.GetUserId()).ToList();
            }
            else
            {
                var pageManager = new ApplicationPageManager(filterContext.HttpContext.Request.GetOwinContext());
                navbarItems = pageManager.GetAnonymousPages().ToList();
            }
            filterContext.HttpContext.Items["NavbarItems"] = navbarItems;
            filterContext.Controller.ViewBag.NavbarItems = navbarItems;
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var rd = filterContext.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action").ToLower();
            string currentController = rd.GetRequiredString("controller").ToLower();
            
            if ((string.Equals(currentAction.ToLower(), "verifyphonenumber") && string.Equals(currentController.ToLower(), "manage") )||
                (string.Equals(currentAction.ToLower(), "logoff") && string.Equals(currentController.ToLower(), "account")) ||
                (string.Equals(currentController.ToLower(), "home")) )
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            var currentUser = filterContext.HttpContext.User;
            if (currentUser.Identity.IsAuthenticated)
            {
                var isAdmin = currentUser.IsInRole(AppConstants.AdminRoleName);
                var userManager = filterContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                if (!isAdmin && !userManager.IsPhoneNumberConfirmed(currentUser.Identity.GetUserId()))
                {
                    var currentUserObj = userManager.FindById(currentUser.Identity.GetUserId());

                    filterContext.Controller.TempData["AddedPhoneNumber"] = currentUserObj.PhoneNumber;
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Manage", action = "VerifyPhoneNumber", PhoneNumber = currentUserObj.PhoneNumber }));
                }
            }
            base.OnActionExecuting(filterContext);
        }


    }

}