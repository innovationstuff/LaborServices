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
using Newtonsoft.Json;
using Westwind.Globalization;

namespace LaborServices.Web.Controllers
{
    [Authorize]
    public class LeadController : BaseController
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

        // GET: Lead
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BusinessSector()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BusinessSector(BusinessLeadViewModel model)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            model.UserId = currentUser.CrmUserId;
            model.SectorId = ((byte)SectorsTypeEnum.Business).ToString();

            if (!ModelState.IsValid) return View(model);

            var requiredJobs = JsonConvert.DeserializeObject<List<RequiredJobsViewModel>>(model.Description);

            StringBuilder sb = new StringBuilder();
            sb.Append("العميل يريد :");
            sb.Append("\n");

            foreach (var requiredJob in requiredJobs)
            {
                string salaryText = "راتب";
                sb.AppendFormat("{0}  {1} {2} {3} {4}", requiredJob.EmpsCount, requiredJob.Job, requiredJob.Nationality, salaryText, requiredJob.Salary);
                sb.Append("\n");
            }
            model.Description = sb.ToString();
            var result = await PostResourceAsync<BusinessLeadViewModel>("api/Lead/Business/Create", model);

            if (result.StatusCode == HttpStatusCode.OK)
                return View("Success", SuccessLeadRequest());


            ModelState.AddModelError("", result.StatusCode == HttpStatusCode.BadRequest ? result.StatusMessage : "Something went wrong");
            return View(model);
        }

        protected ResultMessageVM SuccessLeadRequest()
        {
           return new ResultMessageVM()
            {
                Title = DbRes.T("SuccessLeadRequest", "LeadResources"),
                Message = DbRes.T("SuccessLeadRequestMsg", "LeadResources"),
                IsWithAutoRedirect = true,
                UrlToRedirect = Url.Action("Index", "Home", new { lang = LangCode }),
                RedirectTimeout = 10
            };
        }


        #region IndividualSector

        public async Task<ActionResult> IndividualSector()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var result = await GetResourceAsync<dynamic>("api/contact/" + currentUser.CrmUserId);
            if (result == null) return HttpNotFound();

            ContactViewModel contact = result.ToObject<ContactViewModel>();
            var model = new IndividualLeadViewModel(contact);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndividualSector(IndividualLeadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var result_ = await PostResourceAsync<bool>("api/Lead/Individual/GetLeadsByMobile", model);


            if (!result_.Result)
            {

                if (!await IsProfileCompleted())
                {
                    var currentUser = UserManager.FindById(User.Identity.GetUserId());

                    var contactResult = await GetResourceAsync<dynamic>("api/contact/" + currentUser.CrmUserId);
                    ContactViewModel contact = contactResult.ToObject<ContactViewModel>();

                    contact.CityId = model.CityId ?? contact.CityId;
                    contact.IdNumber = model.IdNumber ?? contact.IdNumber;
                    contact.JobTitle = model.JobTitle ?? contact.JobTitle;
                    contact.RegionId = model.RegionId ?? contact.RegionId;

                    contact.Email = model.Email ?? contact.Email;
                    contact.NationalityId = model.NationalityId ?? contact.NationalityId;
                    contact.GenderId = model.GenderId ?? contact.GenderId;

                    await PostResourceAsync<ContactViewModel>("api/contact/UpdateProfile", contact);
                }

                var result = await PostResourceAsync<BusinessLeadViewModel>("api/Lead/Individual/Create", model);

                if (result.StatusCode == HttpStatusCode.OK)
                    return View("Success", SuccessLeadRequest());

                ModelState.AddModelError("", result.StatusCode == HttpStatusCode.BadRequest ? result.StatusMessage : "Something went wrong");
                return View(model);
            }
            else
            {
                ViewBag.ErrorMessage = Lang == Language.Arabic ? "هذا العميل لديه طلبـات سابقة" : "This client has prior requests";
                return View(model);
            }




        }


        private async Task<bool> IsProfileCompleted()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (string.IsNullOrEmpty(currentUser.CrmUserId))
                return false;

            var isProfileCompleted = await GetResourceAsync<dynamic>("api/contact/IsProfileCompleted/" + currentUser.CrmUserId);
            var result = isProfileCompleted as bool?;
            return result != null && result != false;
        }

        #endregion


        #region caling api

        [AllowAnonymous]
        public async Task<JsonResult> GetRegions()
        {
            var data = await GetResourceAsync<dynamic>("api/Lead/Options/Regions");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetIndustryCodes()
        {
            var data = await GetResourceAsync<dynamic>("api/Lead/Options/IndustryCodes");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}