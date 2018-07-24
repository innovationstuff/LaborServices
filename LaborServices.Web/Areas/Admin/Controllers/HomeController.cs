using LaborServices.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        [SetPermissions(nameAr: "لوحة التحكم", nameEn: "Dashboard", controller: "Home", action: "Index", area: "Admin", isBaseParent: false)]
        public ActionResult Index()
        {
            return View();
        }
		public PartialViewResult Charts()
		{
			return PartialView("Shared/_Charts");
		}
    }
}