using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LaborServices.Managers.Identity;
using LaborServices.Utility;
using LaborServices.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LaborServices.Web.Controllers
{
    [Authorize]
    public class ComplaintsController : BaseController
    {

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Complaints
        public ActionResult Index()
        {
            return View();
        }


        #region  dalal

        public ActionResult Dalal()
        {
            return View();
        }

        public async Task<ActionResult> DalalList(string status)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("api/CustomerTicket/Dalal/GetTickets?sectorId=4&userId={0}", currentUser.CrmUserId);

            if (!string.IsNullOrEmpty(status))
                urlBuilder.AppendFormat("&statusCode={0}", status);

            var result = await GetResourceAsync<List<CustomerTicket>>(urlBuilder.ToString());
            return PartialView("_DalalList", result);
        }

        public async Task<ActionResult> DalalDetails(string id = "")
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            string url = string.Format("api/CustomerTicket/Dalal/GetTicket/{0}", id);
            var details = await GetResourceAsync<CustomerTicket>(url);

            if (details == null) return HttpNotFound();
            return View(details);
        }

        public ActionResult CreateDalal()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            CustomerTicket model = new CustomerTicket
            {
                ContactId = currentUser.CrmUserId,
                SectorTypeId = "4"
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDalal(CustomerTicket model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProblemTypeId == ((int) (ProblemTypes.ComplainOnEmployee)).ToString())
                {
                    if (string.IsNullOrEmpty(model.EmployeeId))
                    {
                        ModelState.AddModelError("EmployeeId", "you must select the worker you complaint about");
                        return View(model);
                    }
                }

                var result = await PostResourceAsync<CustomerTicket>("api/CustomerTicket/Dalal/Create", model);

                if (result.StatusCode == HttpStatusCode.OK)
                    return RedirectToAction("Dalal");
                if (result.StatusCode == HttpStatusCode.InternalServerError)
                {
                    ModelState.AddModelError("", "failed send complaint");
                    return View(model);
                }

                ModelState.AddModelError("", result.StatusMessage);
                return View(model);
            }
            return View(model);
        }

        #endregion

        public ActionResult Individuals()
        {
            return View();
        }


        [AllowAnonymous]
        public async Task<JsonResult> GetStatus()
        {
            var data = await GetResourceAsync<dynamic>("api/CustomerTicket/Options/StatusList");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetUserContracts()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var data = await GetResourceAsync<dynamic>("api/CustomerTicket/Dalal/Contracts?userId=" + currentUser.CrmUserId);
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetProblemTypes()
        {
            var data = await GetResourceAsync<dynamic>("api/CustomerTicket/Dalal/ProblemTypes");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetCustomerServedWorkers()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var data = await GetResourceAsync<dynamic>("api/CustomerTicket/Dalal/ServedEmployees?userId=" + currentUser.CrmUserId);
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

       
   
    }
}